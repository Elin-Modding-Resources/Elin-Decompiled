using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Applibot;

public class SpriteVertexPositionChanger : MonoBehaviour
{
	public float scale = 1.5f;

	[NonSerialized]
	private Image _image;

	private void Start()
	{
		_image = GetComponent<Image>();
		if (_image == null)
		{
			Debug.LogError("Imageコンポーネントが必要です");
			return;
		}
		_image.useSpriteMesh = true;
		Sprite sprite = _image.sprite;
		if (sprite.packed)
		{
			_image.rectTransform.sizeDelta *= scale;
		}
		ChangeMeshScale(sprite);
	}

	private void ChangeMeshScale(Sprite sprite)
	{
		NativeSlice<Vector3> vertexAttribute = sprite.GetVertexAttribute<Vector3>(VertexAttribute.Position);
		NativeArray<Vector3> src = new NativeArray<Vector3>(vertexAttribute.Length, Allocator.Temp);
		for (int i = 0; i < vertexAttribute.Length; i++)
		{
			src[i] = vertexAttribute[i] * scale;
		}
		sprite.SetVertexAttribute(VertexAttribute.Position, src);
		src.Dispose();
	}
}
