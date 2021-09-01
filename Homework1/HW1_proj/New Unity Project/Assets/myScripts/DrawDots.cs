using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Created by Vijay Kalivarapu, Iowa State University
 * ME 557 HW 1 Template - OpenGL based Dots
 * 
 * There are three dots rendered in the Game window when 
 * you hit play in Unity
 * HW1: You will need to do the following:
 * - Add a fourth dot using MakeADot()
 * - Display your initials using only GL calls
 *     - You can reuse/modify MakeADot() or create another function that
 *       uses GL lines
 * - Explore and get comfortable with the Unity editor. If you think
 *   you can add some cool features using GL or Unity built-in functions,
 *   go ahead and demonstrate your scripting prowess.
 * 
 * 
 * */

public class DrawDots : MonoBehaviour
{

	static Material glMaterial;
	private Vector2 initialsCurOffset = new Vector2(0f,0f);

	// Start is called before the first frame update
	void Start()
	{
		CreateGLMaterial();
		
	}

	private void OnRenderObject()
	{
		glMaterial.SetPass(0);

		/**
		 * 3a Creates four dots
		 * 
		 */
		makeADot(new Vector2(-2.0f, -2.0f));
		makeADot(new Vector2(2.0f, -2.0f));
		makeADot(new Vector2(2.0f, 2.0f));
		makeADot(new Vector2(-2.0f, 2.0f));

		/**
		 * 3b Creates initials JM for Justin Merkel
		 */
		drawLetterJ(new Vector2(-0.75f + initialsCurOffset.x, 0.75f + initialsCurOffset.y));
		drawLetterM(new Vector2(0.25f + initialsCurOffset.x, -0.75f + initialsCurOffset.y));

		

	}

	void Update()
	{
		/*
		 * 3c Explore Unity Editor
		 */
		Vector2 mousePos = Input.mousePosition;
		mousePos.x = mousePos.x / Screen.width;
		mousePos.y = mousePos.y / Screen.height;
		initialsCurOffset = new Vector2((mousePos.x + 499*initialsCurOffset.x)/500, (mousePos.y + 499*initialsCurOffset.y)/500);
		//Debug.Log(initialsCurOffset);
		float rotationMagnitude = 25f;
		transform.Rotate(new Vector3(mousePos.x * Time.deltaTime * rotationMagnitude
			, mousePos.y * Time.deltaTime* rotationMagnitude));
	}

	private void drawLetterM(Vector2 offset)
	{
		makeALine(new Vector2(0.0f + offset.x, 0.0f + offset.y), new Vector2(0.0f + offset.x, 1.5f + offset.y));
		makeALine(new Vector2(0.0f + offset.x, 1.5f + offset.y), new Vector2(0.6f + offset.x, 0.5f + offset.y));
		makeALine(new Vector2(0.6f + offset.x, 0.5f + offset.y), new Vector2(1.2f + offset.x, 1.5f + offset.y));
		makeALine(new Vector2(1.2f + offset.x, 1.5f + offset.y), new Vector2(1.2f + offset.x, 0f + offset.y));
	}

	private void drawLetterJ(Vector2 offset)
	{
		makeALine(new Vector2(-0.8f + offset.x, 0f + offset.y), new Vector2(0.8f + offset.x, 0f + offset.y));
		//makeALine(new Vector2(0f, 0f), new Vector2(0f, -0.5f));
		makeABezierCurve(new Vector2(0f + offset.x, 0f + offset.y), new Vector2(-0.75f + offset.x, -1.25f + offset.y), new Vector2(0f + offset.x, -2.25f + offset.y), 40);
	}

	private void makeABezierCurve(Vector2 start, Vector2 end, Vector2 mid, int vertices)
	{

		Vector2 curPoint = start;
		for (int i = 0; i < vertices; i++)
		{
			double t = (1.0 / vertices) * (i + 1);
			//Debug.Log(t);
			Vector2 nextPoint = new Vector2(0, 0);
			nextPoint.x = (float)(Math.Pow(1 - t, 2.0)* start.x + 2*(1 - t)*t*mid.x + Math.Pow(t, 2.0) * end.x);
			nextPoint.y = (float)(Math.Pow(1 - t, 2.0) * start.y + 2 * (1 - t) * t * mid.y + Math.Pow(t, 2.0) * end.y);
			makeALine(curPoint, nextPoint);
			curPoint = nextPoint;
			
		}
	}

		private void makeALine(Vector2 start, Vector2 end)
	{
		GL.PushMatrix();
		GL.MultMatrix(transform.localToWorldMatrix);

		GL.Begin(GL.LINES);
		GL.Color(Color.green);
		GL.Vertex(start);
		GL.Vertex(end);
		GL.End();

		GL.PopMatrix();
	}

	private void makeADot(Vector2 pos)
	{
		GL.PushMatrix();
		GL.MultMatrix(transform.localToWorldMatrix);

		GL.Begin(GL.QUADS);
		GL.Color(Color.red);
		GL.Vertex(new Vector2(pos.x, pos.y));
		GL.Vertex(new Vector2(pos.x + 0.2f, pos.y));
		GL.Vertex(new Vector2(pos.x + 0.2f, pos.y + 0.2f));
		GL.Vertex(new Vector2(pos.x, pos.y + 0.2f));
		GL.End();

		GL.PopMatrix();


	}

	static void CreateGLMaterial()
	{
		if (!glMaterial)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			glMaterial = new Material(shader);
			glMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			glMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			glMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			glMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			glMaterial.SetInt("_ZWrite", 0);
		}
	}



}
