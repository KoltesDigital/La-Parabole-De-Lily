using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
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

	private Vector2 previousMousePosition = Vector2.zero;
	private Shape draggedBlock = null;

	private void Spawn(List<Shape> list, Sprite[] sprites, int type)
	{
		var instance = Instantiate(shapePrefab, transform);

		var shape = instance.GetComponent<Shape>();
		list.Add(shape);

		shape.type = type;
		shape.sprite = sprites[type];
		shape.position = new Vector2(Random.Range(0f, screenSize.x), Random.Range(0f, screenSize.y));
	}

	private void Spawn()
	{
		int type = Random.Range(0, blockSprites.Length);
		Spawn(blocks, blockSprites, type);
		Spawn(collectors, collectorsSprites, type);
	}

	void Start()
	{
		Spawn();
	}

	void Update()
	{
		var viewportPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		var mousePosition = new Vector2(viewportPosition.x * screenSize.x, viewportPosition.y * screenSize.y);

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

				draggedBlock.Hide();
				droppedCollector.Hide();
			}
			draggedBlock = null;
		}

		if (draggedBlock != null)
		{
			draggedBlock.position += mousePosition - previousMousePosition;
		}

		previousMousePosition = mousePosition;
	}
}
