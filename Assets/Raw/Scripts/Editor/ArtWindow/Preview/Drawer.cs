using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TriangleNet.Data;
using TriangleNet.Tools;
using TriangleNet.Geometry;
using EditorWindowTools;

namespace RuntimeArtWay {
public class Drawer : AbstractEditorTool<Sample> {

	public Drawer(){

	}

	private bool isDrawing;
	private Rect rect;

	public void Draw(Rect rect){
		this.rect = rect;

		Draw();
	}

	protected override void OnDraw(){
		var e = Event.current;
		
		if (!e.isMouse) {
			return;
		}

		switch (e.type){
			case EventType.MouseDown: {
				MouseDown(e.mousePosition);
				break;
			}
			case EventType.MouseDrag: {
				MouseDrag(e.mousePosition);
				break;
			}
			case EventType.MouseUp: {
				MouseUp(e.mousePosition);
				break;
			}
			default: {
				Debug.Log("Hmmmm " + e.type);
				break;
			}
		}

	}

	private void MouseDown(Vector2 mousePosition){
		if (target.IsDrawn) return;
		if (isDrawing) MouseUp(mousePosition);

		if (!rect.Contains(mousePosition)) return;

		target.verticles.Add(mousePosition);
		isDrawing = true;
		Debug.Log("Begin");
	}

	private void MouseDrag(Vector2 mousePosition){
		if (!isDrawing) return;
		target.verticles.Add(mousePosition);
		Debug.Log("Update");
	}

	private void MouseUp(Vector2 mousePosition){
		target.equalDistance = EqualDistanceUtil.Prepare(target.verticles, 1f);
		isDrawing = false;
		Debug.Log("Finished");
	}

}
}