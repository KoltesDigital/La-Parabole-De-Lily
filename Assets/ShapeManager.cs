using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
	static public ShapeManager instance { get; private set; }

	[SerializeField]
	private float screenHeight = 1080f;

	[SerializeField]
	private GameObject shapePrefab;

	[SerializeField]
	private Sprite[] blockSprites;

	[SerializeField]
	private Sprite[] collectorsSprites;

	private List<Shape> blocks = new List<Shape>();
	private List<Shape> collectors = new List<Shape>();

	[SerializeField]
	private float dragThreshold;

	[SerializeField]
	private float dropThreshold;

	[SerializeField]
	private float spawnDistance;

	[SerializeField]
	private float margin;

	[SerializeField]
	private float avoidDistance;

	[SerializeField]
	private float avoidRatio;

	[SerializeField]
	private float avoidMaxForce;

	private Vector2 previousMousePosition = Vector2.zero;
	private Shape draggedBlock = null;

	private GameChapterData chapter;
	private GameSettings settings;

	private int successCount;
	private int failureCount;

	private float screenRatio
	{
		get
		{
			return Screen.width / (float)Screen.height;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Spawn(List<Shape> list, Sprite[] sprites, int type)
	{
		var instance = Instantiate(shapePrefab, transform);

		var shape = instance.GetComponent<Shape>();

		shape.type = type;
		shape.sprite = sprites[type];

		for (; ; )
		{
			shape.position = new Vector2(
				Random.Range(-screenRatio * screenHeight * .5f + margin, screenRatio * screenHeight * .5f - margin),
				Random.Range(-screenHeight * .5f + margin, screenHeight * .5f - margin));

			if (blocks
				.Where(other => Vector2.Distance(shape.position, other.position) < spawnDistance)
				.Count() > 0)
			{
				continue;
			}

			if (collectors
				.Where(other => Vector2.Distance(shape.position, other.position) < spawnDistance)
				.Count() > 0)
			{
				continue;
			}

			break;
		}

		if (settings.tutorial)
		{
			if (list == blocks)
			{
				shape.position = new Vector2(-400f, 0f);
			}
			if (list == collectors)
			{
				shape.position = new Vector2(400f, 0f);
			}
		}

		list.Add(shape);
	}

	private void Spawn()
	{
		int type;

		for (; ; )
		{
			type = Random.Range(0, blockSprites.Length);

			if (blocks
				.Where(other => other.type == type)
				.Count() > 0)
			{
				continue;
			}
			break;
		}

		Spawn(blocks, blockSprites, type);
		Spawn(collectors, collectorsSprites, type);
	}

	public void Initialize(GameChapterData data)
	{
		chapter = data;
		settings = data.settings;

		successCount = 0;
		failureCount = 0;

		for (int i = 0; i < settings.shapeCount; ++i)
		{
			--settings.shapeRemainder;
			Spawn();
		}
	}

	void Update()
	{
		var viewportPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		var mousePosition = new Vector2(
			(viewportPosition.x - .5f) * screenRatio * screenHeight,
			(viewportPosition.y - .5f) * screenHeight);

		if (blocks.Count > 0)
		{
			if (Input.GetMouseButtonDown(0))
			{
				var draggedTuple = blocks
					.Select(shape => new System.Tuple<Shape, float>(shape, Vector2.Distance(mousePosition, shape.position)))
					.Where(tuple => tuple.Item2 < dragThreshold)
					.OrderBy(tuple => tuple.Item2)
					.FirstOrDefault();

				if (draggedTuple != null)
				{
					draggedBlock = draggedTuple.Item1;
				}
			}

			if (Input.GetMouseButtonUp(0) && draggedBlock != null)
			{
				var droppedTuple = collectors
					.Select(shape => new System.Tuple<Shape, float>(shape, Vector2.Distance(draggedBlock.position, shape.position)))
					.Where(tuple => tuple.Item2 < dropThreshold)
					.OrderBy(tuple => tuple.Item2)
					.FirstOrDefault();

				if (droppedTuple != null)
				{
					var droppedCollector = droppedTuple.Item1;

					if (draggedBlock.type == droppedCollector.type)
					{
						++successCount;
					}
					else
					{
						++failureCount;
					}

					draggedBlock.Hide();
					blocks.Remove(draggedBlock);

					droppedCollector.Hide();
					collectors.Remove(droppedCollector);

					if (settings.shapeRemainder > 0)
					{
						--settings.shapeRemainder;
						Spawn();
					}

					if (blocks.Count == 0)
					{
						if (failureCount < 2)
						{
							StoryManager.instance.OpenChapter(chapter.nextChapterOnSuccess);
						}
						else if (successCount < 2)
						{
							StoryManager.instance.OpenChapter(chapter.nextChapterOnFailure);
						}
						else
						{
							StoryManager.instance.OpenChapter(chapter.nextChapterOnMiddle);
						}
					}
				}

				draggedBlock = null;
			}

			if (draggedBlock != null)
			{
				draggedBlock.position += mousePosition - previousMousePosition;

				if (settings.avoidsMismatch)
				{
					foreach (var collector in collectors)
					{
						if (collector.type != draggedBlock.type)
						{
							var offsetBlock = collector.position - draggedBlock.position;
							var d2 = offsetBlock.sqrMagnitude;

							Vector2 force = offsetBlock * Mathf.Min(avoidRatio / d2, avoidMaxForce);

							collector.position += force * Time.deltaTime;
							collector.position = new Vector2(
								Mathf.Clamp(collector.position.x, -screenRatio * screenHeight * .5f + margin, screenRatio * screenHeight * .5f - margin),
								Mathf.Clamp(collector.position.y, -screenHeight * .5f + margin, screenHeight * .5f - margin));
						}
					}
				}
			}
		}

		previousMousePosition = mousePosition;
	}
}
