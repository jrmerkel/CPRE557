using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Created by Vijay Kalivarapu, Iowa State University
 * 
 * You are given a template and a canvas UI to draw some 2D shapes
 * Use the template to add features you were asked in the homework
 * 
 * HW2: 
 * 
 * Milestone 1: Open Unity executable and display a menu allowing a user to clear, quit, and 
 *	draw points by clicking in the window with a left mouse button. Test to ensure 
 *	that each of these functions can be done repeatedly, in random order. 
 *	
 * Milestone 2: In addition to fulfilling all of the functions required for milestone 1, 
 * you must also allow the user to draw LINES between points selected with 
 * two successive clicks of the left mouse button, SCRIBBLE freehand,
 * and change LINE WIDTH, PIXEL SIZE and COLOR using the given dropdown UI template and scripts
 * 
 * Milestone 3: In addition to fulfilling all of the functions required for milestone 2,
 * this executable must also allow the user to draw FILLED or outlined RECTANGLES whose 
 * opposite corners are specified with two successive clicks of the left mouse button.
 * 
 * */

public class ShapesTemplate : MonoBehaviour
{

	static Material glMaterial;

	//private List<Vector2> _drawPts;


	// Create a reference variable to the Canvas UI
	private Canvas _myCanvas;

	// Variables to store the UI states
	private bool _clearScreen = false;
	private int _colorPick = 0; //0: red, 1: green, 2: yellow
	private int _pickShape = 0; //0: points, 1: lines, 2: rectangles, 3: freewheeling
	private int _eraseShape = 0;

	private Color _colorVal = Color.red;
	private float _pixelSizeVal = 0.1f;

	// Data structure for Dots
	struct DOTS
	{
		public Vector2 pts;
		public Color col;
		public float size;
	};

	private List<Shape> _shapeList;

	private bool shapeFlag;


	// Start is called before the first frame update
	void Start()
	{
		CreateGLMaterial();

		_myCanvas = GameObject.Find("CanvasUI").GetComponent<Canvas>();

		_shapeList = new List<Shape>();
		
	}

	// Update is called once per frame
	private void Update()
	{
		// To prevent capturing mouse click events when a UI element is clicked on
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			switch (_pickShape)
			{
				case 0: // set up dots struct instance
					if (Input.GetMouseButtonDown(0))
					{
						_shapeList.Add(new Dot(_pixelSizeVal, Camera.main.ScreenToWorldPoint(Input.mousePosition), _colorVal, transform));
					}
					break;

				case 1: // Lines
					if (Input.GetMouseButtonDown(0))
					{
						if (!shapeFlag)
						{
							_shapeList.Add(new Line(_pixelSizeVal, Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(Input.mousePosition), _colorVal, transform));
							shapeFlag = true;

						}
						else
						{
							_shapeList[(_shapeList.Count - 1)].AddPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
							shapeFlag = false;
						}
					}
					break;

				case 2: // Rectangle
					if (Input.GetMouseButtonDown(0))
					{
						if (!shapeFlag)
						{
							_shapeList.Add(new Rectangle(_pixelSizeVal, Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(Input.mousePosition), _colorVal, transform));
							shapeFlag = true;

						}
						else
						{
							_shapeList[(_shapeList.Count - 1)].AddPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
							shapeFlag = false;
						}
					}
					break;

				case 3: //Scrib
					if(Input.GetKey(KeyCode.Mouse0))
                    {
						if(!shapeFlag)
						{
							_shapeList.Add(new Scribble(_pixelSizeVal, Camera.main.ScreenToWorldPoint(Input.mousePosition), _colorVal, transform));
							shapeFlag = true;

						}
						else
						{
							_shapeList[(_shapeList.Count - 1)].AddPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
							
						}
					}
                    else
                    {
						shapeFlag = false;
					}
					break;

				default:
					if (Input.GetMouseButtonDown(0))
					{
						_shapeList.Add(new Dot(_pixelSizeVal, Camera.main.ScreenToWorldPoint(Input.mousePosition), _colorVal, transform));
					}
					break;
			}
		}
	}

	private void OnRenderObject()
	{
		if (_clearScreen == true)
			GL.Clear(true, true, Color.black, 1.0f);

		glMaterial.SetPass(0);

		// Draw all primitives

		foreach (Shape s in _shapeList)
		{
			s.DrawShape();
		}

	}

	static void CreateGLMaterial()
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


	public void ClearScreenButton()
	{
		_clearScreen = true;

		// clear screen
		GL.Clear(true, true, Color.black, 1.0f);

		// empty out _dots struct
		_shapeList.Clear();

		// empty out other shapes

	}

	public void ColorPickDropdown()
	{
		_colorPick = GameObject.Find("Dropdown Color Pick").GetComponent<Dropdown>().value;
		switch(_colorPick)
		{
			case 0:
				_colorVal = Color.red;
				break;
			case 1:
				_colorVal = Color.green;
				break;
			case 2:
				_colorVal = Color.blue;
				break;
			default:
				_colorVal = new Color(1.0f, 1.0f, 1.0f);	//white
				break;
		}
	}

	public void PixelSizeSlider()
	{
		_pixelSizeVal = GameObject.Find("Pixel_Size_Slider").GetComponent<Slider>().value;
	}

	public void ShapePickDropdown()
	{
		_pickShape = GameObject.Find("Dropdown Draw a shape").GetComponent<Dropdown>().value;
		shapeFlag = false;
	}


	public void ErasePickDropdown()
	{
		_eraseShape = GameObject.Find("Dropdown_Erase").GetComponent<Dropdown>().value;
	}

	public void EraseButton()
    {
		//Determine runtime type of each Shape and if it matches the erase type remove from list
		if (_eraseShape == 0)
		{
			_shapeList.RemoveAll(s => s is Dot);
		}
		else if (_eraseShape == 1)
		{
			_shapeList.RemoveAll(s => s is Line);
		}
		else if (_eraseShape == 2)
		{
			_shapeList.RemoveAll(s => s is Rectangle);
		}
		else if (_eraseShape == 3)
		{
			_shapeList.RemoveAll(s => s is Scribble);
		}
		
    }
	public void UndoButton()
    {
		if(_shapeList.Count > 0)
			_shapeList.RemoveAt(_shapeList.Count - 1);
    }

	public void QuitButton()
	{
		Application.Quit();
	}

	public abstract class Shape
    {
		public abstract void DrawShape();
		public abstract void AddPoint(Vector2 v);

		public void drawPoint(Vector2 loc, Transform t, Color c, float dotsize)
		{
			GL.PushMatrix();
			GL.MultMatrix(t.localToWorldMatrix);

			GL.Begin(GL.QUADS);
			GL.Color(c);
			GL.Vertex(new Vector2(loc.x - dotsize / 2.0f, loc.y - dotsize / 2.0f));
			GL.Vertex(new Vector2(loc.x + dotsize / 2.0f, loc.y - dotsize / 2.0f));
			GL.Vertex(new Vector2(loc.x + dotsize / 2.0f, loc.y + dotsize / 2.0f));
			GL.Vertex(new Vector2(loc.x - dotsize / 2.0f, loc.y + dotsize / 2.0f));
			GL.End();

			GL.PopMatrix();
		}
		public void drawLine(Vector2 start, Vector2 end, Transform t, Color c)
		{
			GL.PushMatrix();
			GL.MultMatrix(t.localToWorldMatrix);

			GL.Begin(GL.LINES);
			GL.Color(c);
			GL.Vertex(new Vector2(start.x, start.y));
			GL.Vertex(new Vector2(end.x, end.y));
			GL.End();

			GL.PopMatrix();
		}
	}

	public class Dot : Shape
    {
		private float m_dotsize;
		private Vector2 m_loc;
		private Color m_col;
		private Transform m_transform;
		public Dot(float dotsize, Vector2 loc, Color col, Transform transform)
        {
			m_dotsize = dotsize;
			m_loc = loc;
			m_col = col;
			m_transform = transform;
        }
		override public void DrawShape()
        {
			drawPoint(m_loc, m_transform, m_col, m_dotsize);
		}
		public override void AddPoint(Vector2 v)
		{
			
		}
	}

	public class Line : Shape
    {
		private float m_dotsize;
		private Vector2 m_start;
		private Vector2 m_end;
		private Color m_col;
		private Transform m_transform;
		public Line(float dotsize, Vector2 start, Vector2 end, Color col, Transform transform)
		{
			m_dotsize = dotsize;
			m_start = start;
			m_end = end;
			m_col = col;
			m_transform = transform;
		}

		public override void DrawShape()
        {
			drawPoint(m_start, m_transform, m_col, m_dotsize);
			drawPoint(m_end, m_transform, m_col, m_dotsize);
			drawLine(m_start, m_end, m_transform, m_col);
		}

		public override void AddPoint(Vector2 v)
        {
			m_end = v;
        }
    }

	public class Rectangle : Shape
    {
		private float m_dotsize;
		private Vector2 m_start;
		private Vector2 m_end;
		private Color m_col;
		private Transform m_transform;

		public Rectangle(float dotsize, Vector2 start, Vector2 end, Color col, Transform transform)
		{
			m_dotsize = dotsize;
			m_start = start;
			m_end = end;
			m_col = col;
			m_transform = transform;
		}
		public override void AddPoint(Vector2 v)
		{
			m_end = v;
		}
		public override void DrawShape()
        {
			drawPoint(m_start, m_transform, m_col, m_dotsize);
			drawPoint(m_end, m_transform, m_col, m_dotsize);
			drawLine(m_start, new Vector2(m_start.x, m_end.y), m_transform, m_col);
			drawLine(new Vector2(m_start.x, m_end.y), m_end, m_transform, m_col);
			drawLine(m_end, new Vector2(m_end.x, m_start.y), m_transform, m_col);
			drawLine(new Vector2(m_end.x, m_start.y), m_start, m_transform, m_col);
		}
    }

	public class Scribble : Shape
	{
		private float m_dotsize;
		private List<Vector2> vList;
		private Color m_col;
		private Transform m_transform;

		public Scribble(float dotsize, Vector2 start, Color col, Transform transform)
		{
			m_dotsize = dotsize;
			vList = new List<Vector2>();
			vList.Add(start);
			m_col = col;
			m_transform = transform;
		}
		public override void AddPoint(Vector2 v)
		{
			vList.Add(v);
		}
		public override void DrawShape()
		{
			foreach(Vector2 point in vList)
            {
				drawPoint(point, m_transform, m_col, m_dotsize);
			}
			
		}
	}
}
