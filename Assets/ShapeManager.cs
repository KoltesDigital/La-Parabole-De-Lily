﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
	static public ShapeManager instance { get; private set; }

	[SerializeField]
	private Vector2 screenSize = new Vector2(1920, 1080);

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

	private Vector2 previousMousePosition = Vector2.zero;
	private Shape draggedBlock = null;

	private GameChapterData chapter;
	private GameSettings settings;

	private bool hasSuccess;
	private bool hasFailure;

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
			shape.position = new Vector2(Random.Range(margin, screenSize.x - margin), Random.Range(margin, screenSize.y - margin));

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

		hasSuccess = false;
		hasFailure = false;

		for (int i = 0; i < settings.shapeCount; ++i)
		{
			--settings.shapeRemainder;
			Spawn();
		}
	}

	void Update()
	{
		var viewportPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		var mousePosition = new Vector2(viewportPosition.x * screenSize.x, viewportPosition.y * screenSize.y);

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
						hasSuccess = true;
					}
					else
					{
						hasFailure = true;
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
						if (hasSuccess && hasFailure)
						{
							StoryManager.instance.OpenChapter(chapter.nextChapterOnMiddle);
						}
						else if (hasSuccess)
						{
							StoryManager.instance.OpenChapter(chapter.nextChapterOnSuccess);
						}
						else
						{
							StoryManager.instance.OpenChapter(chapter.nextChapterOnFailure);
						}
					}
				}

				draggedBlock = null;
			}

			if (draggedBlock != null)
			{
				draggedBlock.position += mousePosition - previousMousePosition;
			}
		}

		previousMousePosition = mousePosition;
	}
}
