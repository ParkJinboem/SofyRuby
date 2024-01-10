//Advanced Mobile Paint - engine i API za iscrtavanje tekstura na mobilnim uredjajima  
//baziran na MobilePaint.cs skripti
//minimalna verija Unity-ja koju podrzava je 4.6.1
//verzija 1.0,septembar 2016.
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AdvancedMobilePaint
{
	// list of drawmodes
	public enum DrawMode
	{
		Default,//color vector brush drawing mode
		CustomBrush,//bitmap brush draw mode 
		FloodFill,//flood fill brush draw mode
		Pattern,//pattern vector brush draw mode
		//Stamp// TODO :this draws brush but it does not do Draw line between two brush positions in same gesture
	}
	//brush settings 
	public enum BrushProperties
	{
		Clear,//eraser-reads brush shape and size but sets all pixels transparent 
		Default,//default - reads brush shape and size but sets base color only
		Simple,//simple - sets non transparent brush pixels by copying data from brush
		Pattern//pattern- sets pattern non transparent pixels using brush size and shape
	}
	//mask mode settings
//	public enum LockMaskMode
//	{
//		Simple,//(Default)uses monochrome masks,locks on target color's transparency byte (allowed values are black and transparent) 
//		Multiregion//(Advanced) uses full color masks ,compares all bytes(all colors allowed) UNSUPPORTED AT THE MOMENT
//	}
	//vector brush settings
	public enum VectorBrush
	{
		Circle //default , more modes to be included in future
		//,
		//Square,
	
	}
	/// <summary>
	/// Advanced mobile paint engine.
	/// </summary>
public class AdvancedMobilePaint : MonoBehaviour {
		/// <summary>
		/// Flag koji oznacava da li je interakcija(promena teksture) dozvoljena.
		/// true= moze da boji ,false ne moze
		/// </summary>
		public bool drawEnabled=false;
		/// <summary>
		/// Pokazivac na Undo controller.
		/// </summary>
		public PaintUndoManager undoController;
		/// <summary>
		/// The brush mode.
		///	Setuje mod brush-a.
		/// </summary>
		public BrushProperties brushMode=BrushProperties.Default;
		/// <summary>
		/// The paint layer mask.
		/// </summary>
		public LayerMask paintLayerMask;
		/// <summary>
		/// The create canvas mesh flag.
		/// Setujete ovaj fleg na true ako je ova komponenta u canvasu.
		/// </summary>
		public bool createCanvasMesh=false;
		/// <summary>
		/// The connect brush stokes flag.
		/// </summary>
		public bool connectBrushStokes=true; // if brush moves too fast, then connect them with line. NOTE! Disable this if you are painting to custom mesh
	
		//public bool doInterpolation = false;
		/// <summary>
		/// The color of the paint.
		/// </summary>
		public Color32 paintColor = new Color32(255,0,0,255);
		//public float resolutionScaler = 1.0f; // 1 means screen resolution, 0.5f means half the screen resolution
		public int brushSize = 24; // default brush size
		/// <summary>
		/// The use additive colors flag.
		/// </summary>
		public bool useAdditiveColors = true; // true = alpha adds up slowly, false = 1 click will instantly set alpha to brush or paint color alpha value
		/// <summary>
		/// The brush alpha strength.
		/// </summary>
		public float brushAlphaStrength = 1f; // multiplier to soften brush additive alpha, 0.1f is nice & smooth, 1 = faster
		/// <summary>
		/// The draw mode.
		/// </summary>
		public DrawMode drawMode = DrawMode.CustomBrush; //
		/// <summary>
		/// The use lock area.
		/// </summary>
		public bool useLockArea=false; // locking mask: only paint in area of the color that your click first
		/// <summary>
		/// The use mask layer only.
		/// </summary>
		public bool useMaskLayerOnly = false; // if true, only check pixels from mask layer, not from the painted texture
		/// <summary>
		/// The use threshold.
		/// </summary>
		public bool useThreshold = false;
		/// <summary>
		/// The paint threshold.
		/// </summary>
		public byte paintThreshold = 128; // 0 = only exact match, 255 = match anything
		
		//lock maska za iscrtavanje
		private byte[] lockMaskPixels; // locking mask pixels
		
		/// <summary>
		/// The can draw on black.
		/// </summary>
		public bool canDrawOnBlack=true; // to stop filling on mask black lines, FIXME: not working if its not pure black..
		//ne menjaj! osim ako ne znas sta radis
		
		public string targetTexture = "_MainTex"; // target texture for this material shader (usually _MainTex)
		
		public FilterMode filterMode = FilterMode.Point;
		
		// clear color
		public Color32 clearColor = new Color32(255,255,255,255);
		
		// for using texture on canvas
		/// <summary>
		/// The use mask image.
		/// </summary>
		public bool useMaskImage=false;
		/// <summary>
		/// The mask texture.
		/// </summary>
		public Texture2D maskTex;
		
		// for using custom brushes
		//public bool useCustomBrushes=true;
		/// <summary>
		/// The custom brush texture.
		/// </summary>
		public Texture2D/*[]*/ customBrush;
		/// <summary>
		/// The use custom brush alpha.
		/// </summary>
		public bool useCustomBrushAlpha=true; // true = use alpha from brush, false = use alpha from current paint color
		//public int selectedBrush = 0; // currently selected brush index
		
		//private Color[] customBrushPixels;
		private byte[] customBrushBytes;
		
		private int customBrushWidth;
		private int customBrushHeight;
		private int customBrushWidthHalf;
		//private int customBrushHeightHalf;
		private int texWidthMinusCustomBrushWidth;
		private int texHeightMinusCustomBrushHeight;
		
		// PATTERNS
		/// <summary>
		/// The custom pattern texture.
		/// </summary>
		public  Texture2D customPattern;
		
		private byte[] patternBrushBytes;
		private int customPatternWidth;
		private int customPatternHeight;
		//public int selectedPattern=0;
		// UNDO
		private byte[] undoPixels; // undo buffer
		//private List<byte[]> undoPixels; // undo buffer(s)
		/// <summary>
		/// The undo enabled flag.
		/// </summary>
		public bool undoEnabled = false;
		// undo step used internaly dont change
		UStep   drawUndoStep= null;
		/// <summary>
		/// The draw texture pixels.
		/// </summary>
		[HideInInspector]
		public byte[] pixels; // byte array for texture painting, this is the image that we paint into.
		[HideInInspector]
		public  byte[] maskPixels; // byte array for mask texture
		private byte[] clearPixels; // byte array for clearing texture
		/// <summary>
		/// The drawing texture.
		/// </summary>
		public Texture2D tex; // texture that we paint into (it gets updated from pixels[] array when painted)
		//private Texture2D maskTex; // texture used as a overlay mask
		private int texWidth;
		private int texHeight;
		//private Touch touch; // touch reference
		private Camera cam; // main camera reference
		private RaycastHit hit;
		private bool wentOutside=false;
		//UNUSED KEPT FOR COMPATIBILITY
		private bool usingClearingImage = false; // did we have initial texture as maintexture, then use it as clear pixels array
		
		private Vector2 pixelUV; // with mouse
		private Vector2 pixelUVOld; // with mouse
		
		//private Vector2[] pixelUVs; // mobiles
		//private Vector2[] pixelUVOlds; // mobiles
		
		[HideInInspector]
		public bool textureNeedsUpdate = false; // if we have modified texture

		public Texture2D pattenTexture;
		Sprite imageSprite;
		
		public Transform raySource;
		public bool useAlternativeRay=false;
		
		public bool overrideSprite=false;
		public int drawsToOverride=3;
		public Touch touch; // touch reference
		public Vector2[] pixelUVs; // mobiles
		public Vector2[] pixelUVOlds; // mobiles
		
		public bool multitouchEnabled=false;
		//Vector3 canvasPosition;
		void Awake()
		{	
			//Debug.Log ("CANVAS POSITION "+ transform.position.ToString() + " , "+ gameObject.GetComponent<RectTransform>().anchoredPosition.ToString());
			//canvasPosition=transform.position;
//		#if !UNITY_EDITOR
//			multitouchEnabled=true;
//		#endif 			
			//InitializeEverything();		
		}
		void Start()
		{
			InitializeEverything();	
		}
		public void InitializeEverything() 
		{
			// WARNING: fixed maximum amount of touches, is set to 20 here. Not sure if some device supports more?
			pixelUVs = new Vector2[20];
			pixelUVOlds = new Vector2[20];
			if (createCanvasMesh)
			{
				Debug.Log ("CREATE SCREEN QUAD!");
				CreateCanvasQuad();
				
			}else{ // using existing mesh
				//if (connectBrushStokes) Debug.LogWarning("Custom mesh used, but connectBrushStokes is enabled, it can cause problems on the mesh borders wrapping");				
				if (GetComponent<MeshCollider>()==null) Debug.LogError("MeshCollider is missing, won't be able to raycast to canvas object");
				if (GetComponent<MeshFilter>()==null || GetComponent<MeshFilter>().sharedMesh==null) Debug.LogWarning("Mesh or MeshFilter is missing, won't be able to see the canvas object");
			}
			// create texture
			if (useMaskImage)
			{
				// check if its assigned
				if (maskTex == null)
				{
					Debug.LogWarning("maskImage is not assigned. Setting 'useMaskImage' to false");
					useMaskImage = false;
				}else{
					// Check if we have correct material to use mask image (layer)
					if (GetComponent<Renderer>().material.name.StartsWith("CanvasWithAlpha") || GetComponent<Renderer>().material.name.StartsWith("CanvasDefault"))
					{
						// FIXME: this is bit annoying to compare material names..
						Debug.LogWarning("CanvasWithAlpha and CanvasDefault materials do not support using MaskImage (layer). Disabling 'useMaskImage'");
						Debug.LogWarning("CanvasWithAlpha and CanvasDefault materials do not support using MaskImage (layer). Disabling 'useMaskLayerOnly'");
						useMaskLayerOnly = false;
						
						useMaskImage = false;
						maskTex = null;
					}else{

						texWidth = maskTex.width;
						texHeight = maskTex.height;
						GetComponent<Renderer>().material.SetTexture("_MaskTex", maskTex);
						
					}
				}
				
			}else{	// no mask texture
				// calculate texture size from screen size
				if(tex!=null)
				{
				texWidth=tex.width;
				texHeight=tex.height;
				}
				else
				texWidth = 0;//(int)(Screen.width*resolutionScaler+canvasSizeAdjust.x);
				texHeight =0;// (int)(Screen.height*resolutionScaler+canvasSizeAdjust.y);
			}
			
			// TODO: check if target texture exists
			if (!GetComponent<Renderer>().material.HasProperty(targetTexture)) Debug.LogError("Fatal error: Current shader doesn't have a property: '"+targetTexture+"'");
			
			// we have no texture set for canvas
			if (GetComponent<Renderer>().material.GetTexture(targetTexture)==null)
			{
				// create new texture
				tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
				GetComponent<Renderer>().material.SetTexture(targetTexture, tex);
				
				// init pixels array
				pixels = new byte[texWidth * texHeight * 4];
				
			}else{ // we have canvas texture, then use that as clearing texture
				
				usingClearingImage = true;
				
				texWidth = GetComponent<Renderer>().material.GetTexture(targetTexture).width;
				texHeight = GetComponent<Renderer>().material.GetTexture(targetTexture).height;
				
				// init pixels array
				pixels = new byte[texWidth * texHeight * 4];

				tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
				
				// we keep current maintex and read it as "clear pixels array"
				ReadClearingImage();
				
				GetComponent<Renderer>().material.SetTexture(targetTexture, tex);
			}
			ClearImage();
			
			// set texture modes
			tex.filterMode = filterMode;
			tex.wrapMode = TextureWrapMode.Clamp;
			//tex.wrapMode = TextureWrapMode.Repeat;
			
			if (useMaskImage)
			{
				ReadMaskImage();
			}
			
			// undo system
			if (undoEnabled)
			{
				undoPixels = new byte[texWidth * texHeight * 4];
				System.Array.Copy(pixels,undoPixels,pixels.Length);
			}
			
			// locking mask enabled
			if (useLockArea)
			{
				lockMaskPixels = new byte[texWidth * texHeight * 4];
			}
			ReadCurrentCustomBrush ();
			//Debug.Log ("CANVAS POSITION "+ transform.position.ToString() + " , "+ gameObject.GetComponent<RectTransform>().anchoredPosition.ToString());
		} // InitializeEverything
		
	

	// Use this for initialization
//	void Start () {
//	
//	}
	
	// Update is called once per frame
		void Update () 
		{
			if (drawEnabled) {
				if(!multitouchEnabled)
					MousePaint ();
				else 
					TouchPaint();
				UpdateTexture();
				
			}
		} 
	
	
	//FUNKCIJE
	public int  CountPixelsOfColor(Color32 c)
	{
		int count =0;
		for(int i=0;i<pixels.Length;i+=4)
		{
			if(pixels[i]==c.r && pixels[i+1]==c.g && pixels[i+2]==c.b && pixels[i+3]==c.a)
				count++;
		}
		
		return count;
	}
		public int  CountUnmaskedPixels()
		{
			int count =0;
			//lockMaskPixels[pixel]==1
			//Color32 c= new Color32(255,255.255,0);
			for(int i=0;i<lockMaskPixels.Length;i+=4)
			{
				if(lockMaskPixels[i]==1)
					count++;
			}
			
			return count;
		}
	public void ImmediateDraw(Transform raySource)
	{
			Ray ray= new Ray(raySource.position,new Vector3(0f,0f,1f));
			if (!Physics.Raycast (/*Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position))*/ray, out hit, Mathf.Infinity, paintLayerMask)) {Debug.Log ("WENT OUTSIDE COLLIDER"); return;}
			if(hit.collider!=gameObject.GetComponent<Collider>()) { Debug.Log("HIT SOME OTHER COLLIDER");return;}
			//pixelUVOld = pixelUV; // take previous value, so can compare them
			Vector2 pixelUV_t = hit.textureCoord;
			if(transform.rotation.eulerAngles.y>170f || transform.rotation.eulerAngles.y<-170f)
				pixelUV=new Vector2(1f-pixelUV_t.x,pixelUV_t.y);
			pixelUV_t.x *= texWidth;
			pixelUV_t.y *= texHeight;
			//Debug.Log ("ID sUV"+pixelUV_t);
			//if (wentOutside) {pixelUVOld = pixelUV;wentOutside=false;}
			
			// lets paint where we hit
			switch (drawMode)
			{
			case DrawMode.Default: // drawing
				if(brushMode==BrushProperties.Default)
					DrawCircle((int)pixelUV_t.x, (int)pixelUV_t.y);
				else if (brushMode==BrushProperties.Pattern)
					DrawPatternCircle((int)pixelUV_t.x, (int)pixelUV_t.y);
				break;
			case DrawMode.Pattern: // draw with pattern	
				DrawPatternCircle((int)pixelUV_t.x, (int)pixelUV_t.y);
				break;
			case DrawMode.CustomBrush: // custom brush
				DrawCustomBrush2((int)pixelUV_t.x, (int)pixelUV_t.y);
				break;	
			case DrawMode.FloodFill: // floodfill
			//	if (pixelUVOld == pixelUV) break;
				if (useThreshold)
				{
					if (useMaskLayerOnly)
					{
						FloodFillMaskOnlyWithThreshold((int)pixelUV_t.x, (int)pixelUV_t.y);
					}else{
						FloodFillWithTreshold((int)pixelUV_t.x, (int)pixelUV_t.y);
					}
				}else{
					if (useMaskLayerOnly)
					{
						FloodFillMaskOnly((int)pixelUV_t.x, (int)pixelUV_t.y);
					}else{
						FloodFill((int)pixelUV_t.x, (int)pixelUV_t.y);
					}
				}
				break;		
			default: // unknown mode
				Debug.LogWarning("AMP: Unknown drawing mode:"+drawMode);
				break;
			}
			textureNeedsUpdate=true;
			UpdateTexture();
			
	}
	
	/// <summary>
	/// Funkcija utvrdjuje da li je raycast sa screenPosition-a(npr Input.mousePosition) pogadja nezamaskirani deo teksture na kojoj se crta.
	/// </summary>
	/// <returns><c>true</c> ako raycast pogadja nezamaskirani deo teksture; otherwise, <c>false</c>.</returns>
	/// <param name="screenPosition">Pocetna pozicija ray-a(npr. Input.mousePosition).</param>
	public bool IsRaycastInsideMask(Vector3 screenPosition)
	{

				//Vector3 newX= Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (!Physics.Raycast (Camera.main.ScreenPointToRay(screenPosition), out hit, Mathf.Infinity, paintLayerMask))
		{
			#if UNITY_EDITOR
			Debug.Log("AMP: RAY IS NOT HITTING THIS MESH COLLIDER AT ALL!");
			#endif
			return false;
		}
				
		Vector2 pixelUV1 = hit.textureCoord;
		pixelUV1.x *= texWidth;
		pixelUV1.y *= texHeight;
		int startX1=((int)pixelUV1.x);
		int startY1=((int)pixelUV1.y);
		int pixel1 = (texWidth*startY1+startX1)*4;
		if((pixel1<0|| pixel1>=pixels.Length))//NOTE: Primetiti da exception na ovoj liniji koda ukazuje na to da glavna tekstura za crtanje nije generisana ili ucitana, generisite/ucitajte je pre poziva ove funkcije
		{
			#if UNITY_EDITOR
			Debug.Log("AMP: RAY IS OUT OF BOUNDS OF MASK TEXTURE AND OUTSIDE DRAWABLE AREA");
			#endif
			return false;
		}
		else
			if(lockMaskPixels[pixel1]==1) //NOTE: Primetiti da exception na ovoj liniji koda ukazuje na to da lock maska nije generisana, generisite je pre poziva ove funkcije
			{
				#if UNITY_EDITOR
				Debug.Log("AMP: RAY IS IN BOUNDS OF MASK TEXTURE AND INSIDE DRAWABLE AREA ");
				#endif
				return true;
					
			}
			else
			{
					#if UNITY_EDITOR
					Debug.Log("AMP: RAY IS OUT OF BOUNDS OF MASK BUT INSIDE DRAWABLE AREA");
					#endif
					return false;
			}
		return false;
			
	}//END IsRaycastInsideMask
	/// <summary>
	/// Converts the Sprite to Texture2D.
	/// </summary>
	/// <returns>o Texture2D.</returns>
	/// <param name="sr">Sprite</param>
	public Texture2D ConvertSpriteToTexture2D(Sprite sr)
	{
			
			//sr.sprite = patterns [index];
			Texture2D texTex = new Texture2D ((int)sr.textureRect.width,(int) sr.textureRect.height, TextureFormat.ARGB32, false);
			Color[] tmp = sr.texture.GetPixels((int)sr.textureRect.x, 
			                                             (int)sr.textureRect.y, 
			                                             (int)sr.textureRect.width, 
			                                             (int)sr.textureRect.height,0 );
			texTex.SetPixels( tmp,0 );
			texTex.Apply (false, false);
			return texTex;
	}

		private Vector3 _PosVec;
		public void SetDrawPos(Vector3 _vec) => _PosVec = _vec;

		private Transform drawPoint;
		public void SetDrawPoint(Transform drawPoint)
        {
			this.drawPoint = drawPoint;
		}

	void MousePaint () 
	{
			
			if (Input.GetMouseButtonDown(0)&& Input.touchCount<=1)
			{	
				//Debug.Log ("PAINTING "+ useAlternativeRay);
				if (useLockArea)
				{
					if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) return;
					if(hit.collider!=gameObject.GetComponent<Collider>()) return;
					Input.multiTouchEnabled=false;
					CreateAreaLockMask((int)(hit.textureCoord.x*texWidth), (int)(hit.textureCoord.y*texHeight));
					//Debug.Log("NAIL new step generated");
					if(undoEnabled)
					{
					drawUndoStep= new UStep();
					switch(drawMode)
					{
						case DrawMode.Default:
						drawUndoStep.type=1;
						break;
						case DrawMode.CustomBrush:
						drawUndoStep.type=0;
						break;
						case DrawMode.FloodFill:
							drawUndoStep.type=2;
						break;
						case DrawMode.Pattern:
							drawUndoStep.type=1;
							
						break;
					}
						drawUndoStep.SetStepPropertiesFromEngine(this);
						drawUndoStep.drawCoordinates= new List<Vector2>();
					}					
				}
			}
			
			if (Input.GetMouseButton(0)&& Input.touchCount<=1)
			{
				//Debug.Log ("PAINTING "+ useAlternativeRay);
				// Only if we hit something, then we continue

				if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(drawPoint.position)), out hit, Mathf.Infinity, paintLayerMask)) {wentOutside=true; return;}
				//if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x + _PosVec.x, Input.mousePosition.y + _PosVec.y, Input.mousePosition.z)), out hit, Mathf.Infinity, paintLayerMask)) {wentOutside=true; return;}
				if(hit.collider!=gameObject.GetComponent<Collider>()) {wentOutside=true; return;}
				pixelUVOld = pixelUV; // take previous value, so can compare them
				//transform.lossyScale.x
				
				pixelUV = hit.textureCoord;
				//rotation fix primitive
				if(transform.rotation.eulerAngles.y>170f || transform.rotation.eulerAngles.y<-170f)
				{
					//Debug.Log ("IM ROTATED" + pixelUV.x);
					pixelUV=new Vector2(1f-pixelUV.x,pixelUV.y);
					//Debug.Log ("IM ROTATED FIX" + pixelUV.x);
				}
				//end rotation fix primitive
				pixelUV.x *= texWidth;
				pixelUV.y *= texHeight;
				//Debug.Log ("UV"+pixelUV);
				if (wentOutside) {pixelUVOld = pixelUV;wentOutside=false;}
//				if(useAlternativeRay)
//				{
//					Debug.Log ("ALT RAY");
//				}else
//					Debug.Log ("USE MOUSE POS");
				// lets paint where we hit
				switch (drawMode)
				{
				case DrawMode.Default: // drawing
					if(!useAdditiveColors && (pixelUVOld == pixelUV)) break;
					if(brushMode==BrushProperties.Default)
						DrawCircle((int)pixelUV.x, (int)pixelUV.y);
					else if (brushMode==BrushProperties.Pattern)
						DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y);
					textureNeedsUpdate = true;
					break;
				case DrawMode.Pattern: // draw with pattern	
					if(!useAdditiveColors && (pixelUVOld == pixelUV)) break;
					DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y);
					textureNeedsUpdate = true;
					break;
				case DrawMode.CustomBrush: // custom brush
					if(!useAdditiveColors && (pixelUVOld == pixelUV)) break;
					DrawCustomBrush2((int)pixelUV.x, (int)pixelUV.y);
					textureNeedsUpdate = true;
					break;	
				case DrawMode.FloodFill: // floodfill
					if (pixelUVOld == pixelUV) break;
					if (useThreshold)
					{
						if (useMaskLayerOnly)
						{
							FloodFillMaskOnlyWithThreshold((int)pixelUV.x, (int)pixelUV.y);
						}else{
							FloodFillWithTreshold((int)pixelUV.x, (int)pixelUV.y);
						}
					}else{
						if (useMaskLayerOnly)
						{
							FloodFillMaskOnly((int)pixelUV.x, (int)pixelUV.y);
						}else{
							FloodFill((int)pixelUV.x, (int)pixelUV.y);
						}
					}
					textureNeedsUpdate = true;
					break;		
				default: // unknown mode
					 Debug.LogWarning("AMP: Unknown drawing mode:"+drawMode);
					break;
				}
				if(drawUndoStep==null && undoEnabled)
				{
					drawUndoStep= new UStep();
					switch(drawMode)
					{
					case DrawMode.Default:
						drawUndoStep.type=1;
						break;
					case DrawMode.CustomBrush:
						drawUndoStep.type=0;
						
						break;
					case DrawMode.FloodFill:
						drawUndoStep.type=2;
						break;
					case DrawMode.Pattern:
						drawUndoStep.type=1;
						break;
					}
					drawUndoStep.SetStepPropertiesFromEngine(this);
					drawUndoStep.drawCoordinates= new List<Vector2>();
				}
				if(undoEnabled )
				{
				Vector2 newCoors= new Vector2(pixelUV.x, pixelUV.y);
				drawUndoStep.drawCoordinates.Add (newCoors);
				}
				
			}
			
			if (Input.GetMouseButtonDown(0)&& Input.touchCount<=1)
			{
				// take this position as start position
				//Debug.Log ("PAINTING "+ useAlternativeRay);
				if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) return;
				if(hit.collider!=gameObject.GetComponent<Collider>()) return;
				
				pixelUVOld = pixelUV;
				
			}
			// check distance from previous drawing point and connect them with DrawLine
			if (connectBrushStokes && Vector2.Distance(pixelUV,pixelUVOld)>brushSize && Input.touchCount<=1)
			{
				
				switch (drawMode)
				{
				case DrawMode.Default: // drawing
					#if UNITY_EDITOR
					DrawLine(pixelUVOld, pixelUV);
					#endif
					#if !UNITY_EDITOR 
					if(Input.touchCount==1)
					{
						
						if(Input.GetTouch(0).phase!=TouchPhase.Stationary)
							DrawLine(pixelUVOld, pixelUV);
					}
					
					#endif
					
					break;
					
				case DrawMode.CustomBrush:
					#if UNITY_EDITOR
					DrawLineWithBrush(pixelUVOld, pixelUV);
					#endif
					#if !UNITY_EDITOR 
					if(Input.touchCount==1)
					{
						
						if(Input.GetTouch(0).phase!=TouchPhase.Stationary)
							DrawLineWithBrush(pixelUVOld, pixelUV);
					}
					
					#endif
					break;
					
				case DrawMode.Pattern:
					#if UNITY_EDITOR
					DrawLineWithPattern(pixelUVOld, pixelUV);
					#endif
					#if !UNITY_EDITOR 
					if(Input.touchCount==1)
					{
						
						if(Input.GetTouch(0).phase!=TouchPhase.Stationary)
							DrawLineWithPattern(pixelUVOld, pixelUV);
					}
					
					#endif
					break;
					
				default: // other modes
					break;
				}
				pixelUVOld = pixelUV;
				textureNeedsUpdate = true;
			}
			
			if (Input.GetMouseButtonUp(0)&& Input.touchCount<=1)
			{
				//if (hideUIWhilePainting && !isUIVisible) ShowUI();
				if (drawEnabled && drawUndoStep!=null && undoEnabled) {
					UStep c= new UStep();
					c=drawUndoStep;
					undoController.AddStep (c);
					drawUndoStep=null;
					

				}
			}
	}
	//
		void TouchPaint()
		{
			int i = 0;
			
			while (i < Input.touchCount) 
			{
				touch = Input.GetTouch(i);
				//#if ENABLE_4_6_FEATURES
				//if (useNewUI && eventSystem.IsPointerOverGameObject(touch.fingerId)) return;
				//#endif
				i++;
			}
			
			i=0;
			// loop until all touches are processed
			while (i < Input.touchCount) 
			{
				
				touch = Input.GetTouch(i);
				if (touch.phase == TouchPhase.Began) 
				{
//					#if ENABLE_4_6_FEATURES
//					if (hideUIWhilePainting && isUIVisible) HideUI();
//					#endif
					
					// when starting, grab undo buffer first
//					if (undoEnabled) {
//						System.Array.Copy (pixels, undoPixels, pixels.Length);
//					}
					
					if (useLockArea)
					{
						if (!Physics.Raycast (Camera.main.ScreenPointToRay(touch.position), out hit, Mathf.Infinity, paintLayerMask)) {wentOutside=true; return;}
						
						/*
						pixelUV = hit.textureCoord;
						pixelUV.x *= texWidth;
						pixelUV.y *= texHeight;
						if (wentOutside) {pixelUVOld = pixelUV;wentOutside=false;}
						CreateAreaLockMask((int)pixelUV.x, (int)pixelUV.y);
						*/
						
						pixelUVs[touch.fingerId] = hit.textureCoord;
						pixelUVs[touch.fingerId].x *= texWidth;
						pixelUVs[touch.fingerId].y *= texHeight;
						if (wentOutside) {pixelUVOlds [touch.fingerId] = pixelUVs [touch.fingerId];wentOutside=false;}
						CreateAreaLockMask((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
					}
				}
				// check state
				if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began) 
				{
					
					// do raycast on touch position
					if (Physics.Raycast (Camera.main.ScreenPointToRay (touch.position), out hit, Mathf.Infinity, paintLayerMask)) 
					{
						// take previous value, so can compare them
						pixelUVOlds [touch.fingerId] = pixelUVs [touch.fingerId];
						// get hit texture coordinate
						pixelUVs [touch.fingerId] = hit.textureCoord;
						pixelUVs [touch.fingerId].x *= texWidth;
						pixelUVs [touch.fingerId].y *= texHeight;
						// paint where we hit
						switch (drawMode) 
						{
						case DrawMode.Default:
							//DrawCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							if(brushMode==BrushProperties.Default)
								DrawCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							else if (brushMode==BrushProperties.Pattern)
								DrawPatternCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							textureNeedsUpdate = true;
							break;
							
						case DrawMode.CustomBrush:
							DrawCustomBrush2 ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							textureNeedsUpdate = true;
							break;
							
						case DrawMode.Pattern:
							DrawPatternCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							textureNeedsUpdate = true;
							break;
							
						case DrawMode.FloodFill:
							if (useThreshold)
							{
								if (useMaskLayerOnly)
								{
									FloodFillMaskOnlyWithThreshold((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
								}else{
									FloodFillWithTreshold((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
								}
							}else{
								if (useMaskLayerOnly)
								{
									FloodFillMaskOnly((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
								}else{
									
									FloodFill((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
								}
							}
							textureNeedsUpdate = true;
							break;	
						default:
							// unknown mode
							break;
						}
						// set flag that texture needs to be applied
						//textureNeedsUpdate = true;
					}
				}
				// if we just touched screen, set this finger id texture paint start position to that place
				if (touch.phase == TouchPhase.Began) 
				{
					pixelUVOlds [touch.fingerId] = pixelUVs [touch.fingerId];
				}
				// check distance from previous drawing point
				if (connectBrushStokes && Vector2.Distance (pixelUVs[touch.fingerId], pixelUVOlds[touch.fingerId]) > brushSize) 
				{
//					switch (drawMode) 
//					{
//					case DrawMode.Default:
//						DrawLine (pixelUVOlds[touch.fingerId], pixelUVs[touch.fingerId]);
//						break;
//						
//					case DrawMode.CustomBrush:
//						DrawLineWithBrush(pixelUVOlds[touch.fingerId], pixelUVs[touch.fingerId]);
//						break;
//						
//					case DrawMode.Pattern:
//						DrawLineWithPattern(pixelUVOlds[touch.fingerId], pixelUVs[touch.fingerId]);
//						break;
//						
//					default:
//						// unknown mode, set back to 0?
//						break;
//					}
					switch (drawMode) 
					{
					case DrawMode.Default:
						//DrawCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
						if(brushMode==BrushProperties.Default)
							DrawCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
						else if (brushMode==BrushProperties.Pattern)
							DrawPatternCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
						textureNeedsUpdate = true;
						break;
						
					case DrawMode.CustomBrush:
						DrawCustomBrush2 ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
						textureNeedsUpdate = true;
						break;
						
					case DrawMode.Pattern:
						DrawPatternCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
						textureNeedsUpdate = true;
						break;
						
					case DrawMode.FloodFill:
						if (useThreshold)
						{
							if (useMaskLayerOnly)
							{
								FloodFillMaskOnlyWithThreshold((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							}else{
								FloodFillWithTreshold((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							}
						}else{
							if (useMaskLayerOnly)
							{
								FloodFillMaskOnly((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							}else{
								
								FloodFill((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							}
						}
						textureNeedsUpdate = true;
						break;	
					default:
						// unknown mode
						break;
					}
					//textureNeedsUpdate = true;
					
					pixelUVOlds [touch.fingerId] = pixelUVs [touch.fingerId];
					
				}
				// loop all touches
				i++;
			}
			
//			#if ENABLE_4_6_FEATURES
//			if (Input.touchCount==0)
//			{
//
//			}
//			#endif
			
		}
		
	
	//
	
	
	
	/// <summary>
	///Updejt teksture.
	///U ovoj funkciji mozete dodati kod koji se odnosi na sve teksture koje se istovremeno boje.
	///Po defaultu se samo obradjuje glavna textura. 
	/// </summary>
	void UpdateTexture ()
	{
		if (textureNeedsUpdate) 
			{
				textureNeedsUpdate = false;
				tex.LoadRawTextureData (pixels);
				tex.Apply (false);
				//SVE OSTALE TESTURE MOGU DA SE UPDEJTUJU OVDE
//				if(createCanvasMesh && ! overrideSprite)
//				{
//
//					imageSprite=Sprite.Create(tex,new Rect(0,0,texWidth,texHeight),new Vector2(0.5f,0.5f));	
//
//					gameObject.GetComponent<Image>().sprite=imageSprite; 
//
//					drawsToOverride--;
//					if(drawsToOverride==-1)
//						overrideSprite=true;
//				}
				//

			}
	}
	//koristi se interno za UNDO/REDO potrebe,nema potrebe da koristite ovu funkciju
	public void CopyUndoPixels(byte[] destiniation)
	{
			System.Array.Copy(undoPixels,destiniation,undoPixels.Length);
	}
	/// <summary>
	/// Creates the area lock mask.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void CreateAreaLockMask(int x, int y)
	{
		if (useThreshold) 
		{
			if (useMaskLayerOnly)
			{
				LockAreaFillWithThresholdMaskOnly(x,y);
			}else{
				LockMaskFillWithThreshold(x,y);
			}
		}else{ // no threshold
			if (useMaskLayerOnly)
			{
				LockAreaFillMaskOnly(x,y);
			}else{
				LockAreaFill(x,y);
			}
		}
			//lockMaskCreated = true; // not used yet
	}
	
	
		// main painting function, http://stackoverflow.com/a/24453110
		/// <summary>
		/// Draws the circle.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void DrawCircle(int x,int y)
		{
			//Debug.Log ("DrawCircle "/*+ x+ ","+y*/);
			// clamp brush inside texture
//			if (createCanvasMesh) // TEMPORARY FIX: with a custom sphere mesh, small gap in paint at the end, so must disable clamp on most custom meshes
//			{
//				//x = PaintTools.ClampBrushInt(x,brushSize,texWidth-brushSize);
//				//y = PaintTools.ClampBrushInt(y,brushSize,texHeight-brushSize);
//			}
			
			if (!canDrawOnBlack)
			{
						//Debug.Log ("ITS ON BLACK!");
						if (pixels[(texWidth*y+x)*4]==0 && pixels[(texWidth*y+x)*4+1]==0 && pixels[(texWidth*y+x)*4+2]==0 && pixels[(texWidth*y+x)*4+3]!=0) return;
			}
			
			int pixel = 0;
			
			// draw fast circle: 
			int r2 = brushSize * brushSize;
			int area = r2 << 2;
			int rr = brushSize << 1;
			for (int i = 0; i < area; i++)
			{
				int tx = (i % rr) - brushSize;
				int ty = (i / rr) - brushSize;
				if (tx * tx + ty * ty < r2)
				{
					if (x+tx<0 || y+ty<0 || x+tx>=texWidth || y+ty>=texHeight) {/*Debug.Log ("SKIP THIS PIXEL");*/continue;} // temporary fix for corner painting
					
					
					pixel = (texWidth*(y+ty)+x+tx)*4;
					//pixel = ( texWidth*( (y+ty) % texHeight )+ (x+tx) % texWidth )*4;
					
					if (useAdditiveColors)
					{
						// additive over white also
						if (!useLockArea || (useLockArea && lockMaskPixels[pixel]==1))
						{
							pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel],paintColor.r,paintColor.a/255f*brushAlphaStrength);
							pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],paintColor.g,paintColor.a/255f*brushAlphaStrength);
							pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],paintColor.b,paintColor.a/255f*brushAlphaStrength);
							pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],paintColor.a,paintColor.a/255*brushAlphaStrength);
						}
						
					}else{ // no additive, just paint my colors
						
						if (!useLockArea || (useLockArea && lockMaskPixels[pixel]==1))
						{
							pixels[pixel] = paintColor.r;
							pixels[pixel+1] = paintColor.g;
							pixels[pixel+2] = paintColor.b;
							pixels[pixel+3] = paintColor.a;
						}
						
					} // if additive
				} // if in circle
			} // for area
		} // DrawCircle()
		/// <summary>
		/// Draws the pattern circle.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void DrawPatternCircle(int x,int y)
		{
			//Debug.Log ("DrawPatternCircle "+ x +" ,"+y);
			
			if (!canDrawOnBlack)
			{
				if (pixels[(texWidth*y+x)*4]==0 && pixels[(texWidth*y+x)*4+1]==0 && pixels[(texWidth*y+x)*4+2]==0 && pixels[(texWidth*y+x)*4+3]!=0) return;
			}
			
			int pixel = 0;
			
			// draw fast circle: 
			int r2 = brushSize * brushSize;//povrsina kruga
			int area = r2 << 2;//sve rgb vrednosti koje cine povrsinu kruga -piksela u krugu
			int rr = brushSize << 1;//precnik kruga
			int tx=0;
			int ty=0;
			float yy=0;
			float xx=0;
			int pixel2=0;
			for (int i = 0; i < area; i++)
			{
				/*int*/ tx = (i % rr) - brushSize;
				/*int*/ ty = (i / rr) - brushSize;
				
				if (tx * tx + ty * ty < r2)//(if in circle) 
				{
					if (x+tx<0 || y+ty<0 || x+tx>=texWidth || y+ty>=texHeight) continue; // temporary fix for corner painting
					
					pixel = (texWidth*(y+ty)+x+tx)*4; // << 2
					//if(pixel<0 || pixel>pixels.Length) continue;
					if (useAdditiveColors)
					{
						// additive over white also
						if (!useLockArea || (useLockArea && lockMaskPixels[pixel]==1))
						{
							
							/*float*/ yy = Mathf.Repeat(y+ty,customPatternWidth);
							/*float*/ xx = Mathf.Repeat(x+tx,customPatternWidth);
							/*int*/ pixel2 = (int) Mathf.Repeat( (customPatternWidth*xx+yy)*4, patternBrushBytes.Length);
							pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel], patternBrushBytes[pixel2],patternBrushBytes[pixel2+3]/255f*brushAlphaStrength);
							pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],patternBrushBytes[pixel2+1],patternBrushBytes[pixel2+3]/255f*brushAlphaStrength);
							pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],patternBrushBytes[pixel2+2],patternBrushBytes[pixel2+3]/255f*brushAlphaStrength);
							pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],patternBrushBytes[pixel2+3],patternBrushBytes[pixel2+3]/255f*brushAlphaStrength);
						}
						
					}else{ // no additive, just paint my colors
						
						if (!useLockArea || (useLockArea && lockMaskPixels[pixel]==1))
						{
							// TODO: pattern dynamic scalar value?
							
							/*float*/ yy = Mathf.Repeat(y+ty,customPatternWidth);
							/*float*/ xx = Mathf.Repeat(x+tx,customPatternWidth);//Debug.Log ("P"+xx+","+yy);
							/*int*/ pixel2 = (int) Mathf.Repeat( (customPatternWidth*xx+yy)*4, patternBrushBytes.Length);
							pixels[pixel] = patternBrushBytes[pixel2];//r
							pixels[pixel+1] = patternBrushBytes[pixel2+1];//g
							pixels[pixel+2] = patternBrushBytes[pixel2+2];//b
							
							pixels[pixel+3] = patternBrushBytes[pixel2+3];//a
							//}
						}
						
					} // if additive
				} // if in circle
			} // for area
			
		} // DrawPatternCircle()
		
		#region FLOODFILL
		/// <summary>
		/// Floods the fill mask only.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void FloodFillMaskOnly(int x,int y)
		{
			Debug.Log ("FloodFillMaskOnly");
			// get canvas hit color
			byte hitColorR = maskPixels[ ((texWidth*(y)+x)*4) +0 ];
			byte hitColorG = maskPixels[ ((texWidth*(y)+x)*4) +1 ];
			byte hitColorB = maskPixels[ ((texWidth*(y)+x)*4) +2 ];
			byte hitColorA = maskPixels[ ((texWidth*(y)+x)*4) +3 ];
			
			// early exit if its same color already
			//if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA) return;
			
			if (!canDrawOnBlack)
			{
				if (hitColorA==0) return;
			}
			
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			lockMaskPixels = new byte[texWidth * texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (texWidth*(ptsy-1)+ptsx)*4; // down
					if (lockMaskPixels[pixel]==0
					    && maskPixels[pixel+0]==hitColorR 
					    && maskPixels[pixel+1]==hitColorG 
					    && maskPixels[pixel+2]==hitColorB 
					    && maskPixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						DrawPoint(pixel);
						lockMaskPixels[pixel]=1;
					}
				}
				
				if (ptsx+1<texWidth)
				{
					pixel = (texWidth*ptsy+ptsx+1)*4; // right
					if (lockMaskPixels[pixel]==0
					    && maskPixels[pixel+0]==hitColorR 
					    && maskPixels[pixel+1]==hitColorG 
					    && maskPixels[pixel+2]==hitColorB 
					    && maskPixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						DrawPoint(pixel);
						lockMaskPixels[pixel]=1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (texWidth*ptsy+ptsx-1)*4; // left
					if (lockMaskPixels[pixel]==0
					    && maskPixels[pixel+0]==hitColorR 
					    && maskPixels[pixel+1]==hitColorG 
					    && maskPixels[pixel+2]==hitColorB 
					    && maskPixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						DrawPoint(pixel);
						lockMaskPixels[pixel]=1;
					}
				}
				
				if (ptsy+1<texHeight)
				{
					pixel = (texWidth*(ptsy+1)+ptsx)*4; // up
					if (lockMaskPixels[pixel]==0
					    && maskPixels[pixel+0]==hitColorR 
					    && maskPixels[pixel+1]==hitColorG 
					    && maskPixels[pixel+2]==hitColorB 
					    && maskPixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						DrawPoint(pixel);
						lockMaskPixels[pixel]=1;
					}
				}
			}
		} // floodfill
		
		
		// basic floodfill
		/// <summary>
		/// FloodFill draw.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void FloodFill(int x,int y)
		{
			
			
			Debug.Log ("FloodFill");
			// get canvas hit color
			byte hitColorR = pixels[ ((texWidth*(y)+x)*4) +0 ];
			byte hitColorG = pixels[ ((texWidth*(y)+x)*4) +1 ];
			byte hitColorB = pixels[ ((texWidth*(y)+x)*4) +2 ];
			byte hitColorA = pixels[ ((texWidth*(y)+x)*4) +3 ];
			
			if (!canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			// early exit if its same color already
			if(brushMode!=BrushProperties.Pattern)
			if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA) return;
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (texWidth*(ptsy-1)+ptsx)*4; // down
					if (pixels[pixel+0]==hitColorR 
					    && pixels[pixel+1]==hitColorG 
					    && pixels[pixel+2]==hitColorB 
					    && pixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						DrawPoint(pixel);
						//DrawPoint(ptsx,ptsy-1);
					}


				}
				
				if (ptsx+1<texWidth)
				{
					pixel = (texWidth*ptsy+ptsx+1)*4; // right
					if (pixels[pixel+0]==hitColorR 
					    && pixels[pixel+1]==hitColorG 
					    && pixels[pixel+2]==hitColorB 
					    && pixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						DrawPoint(pixel);
						//DrawPoint(ptsx,ptsy);
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (texWidth*ptsy+ptsx-1)*4; // left
					if (pixels[pixel+0]==hitColorR 
					    && pixels[pixel+1]==hitColorG 
					    && pixels[pixel+2]==hitColorB 
					    && pixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						DrawPoint(pixel);
						//DrawPoint(ptsx-1,ptsy);
					}
				}
				
				if (ptsy+1<texHeight)
				{
					pixel = (texWidth*(ptsy+1)+ptsx)*4; // up
					if (pixels[pixel+0]==hitColorR 
					    && pixels[pixel+1]==hitColorG 
					    && pixels[pixel+2]==hitColorB 
					    && pixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						DrawPoint(pixel);
						//DrawPoint(ptsx,ptsy+1);
					}
				}
			}
		} // floodfill
		/// <summary>
		/// Floodfill by using mask with threshold.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void FloodFillMaskOnlyWithThreshold(int x,int y)
		{
			Debug.Log ("FloodFillMaskOnlyWithThreshold");
			//Debug.Log("hits");
			// get canvas hit color
			byte hitColorR = maskPixels[ ((texWidth*(y)+x)*4) +0 ];
			byte hitColorG = maskPixels[ ((texWidth*(y)+x)*4) +1 ];
			byte hitColorB = maskPixels[ ((texWidth*(y)+x)*4) +2 ];
			byte hitColorA = maskPixels[ ((texWidth*(y)+x)*4) +3 ];
			
			if (!canDrawOnBlack)
			{
				if (hitColorA!=0) return;
			}
			
			// early exit if outside threshold?
			//if (CompareThreshold(paintColor.r,hitColorR) && CompareThreshold(paintColor.g,hitColorG) && CompareThreshold(paintColor.b,hitColorB) && CompareThreshold(paintColor.a,hitColorA)) return;
			if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA) return;
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			lockMaskPixels = new byte[texWidth * texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (texWidth*(ptsy-1)+ptsx)*4; // down
					if (lockMaskPixels[pixel]==0
					    && CompareThreshold(maskPixels[pixel+0],hitColorR) 
					    && CompareThreshold(maskPixels[pixel+1],hitColorG) 
					    && CompareThreshold(maskPixels[pixel+2],hitColorB) 
					    && CompareThreshold(maskPixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						DrawPoint(pixel);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<texWidth)
				{
					pixel = (texWidth*ptsy+ptsx+1)*4; // right
					if (lockMaskPixels[pixel]==0
					    && CompareThreshold(maskPixels[pixel+0],hitColorR) 
					    && CompareThreshold(maskPixels[pixel+1],hitColorG) 
					    && CompareThreshold(maskPixels[pixel+2],hitColorB) 
					    && CompareThreshold(maskPixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						DrawPoint(pixel);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (texWidth*ptsy+ptsx-1)*4; // left
					if (lockMaskPixels[pixel]==0
					    && CompareThreshold(maskPixels[pixel+0],hitColorR) 
					    && CompareThreshold(maskPixels[pixel+1],hitColorG) 
					    && CompareThreshold(maskPixels[pixel+2],hitColorB) 
					    && CompareThreshold(maskPixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						DrawPoint(pixel);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsy+1<texHeight)
				{
					pixel = (texWidth*(ptsy+1)+ptsx)*4; // up
					if (lockMaskPixels[pixel]==0
					    && CompareThreshold(maskPixels[pixel+0],hitColorR) 
					    && CompareThreshold(maskPixels[pixel+1],hitColorG) 
					    && CompareThreshold(maskPixels[pixel+2],hitColorB) 
					    && CompareThreshold(maskPixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						DrawPoint(pixel);
						lockMaskPixels[pixel] = 1;
					}
				}
			}
		} // floodfillWithTreshold
		
		/// <summary>
		/// Floodfill with treshold.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void FloodFillWithTreshold(int x,int y)
		{
			Debug.Log ("FloodFillWithThreshold");
			// get canvas hit color
			byte hitColorR = pixels[ ((texWidth*(y)+x)*4) +0 ];
			byte hitColorG = pixels[ ((texWidth*(y)+x)*4) +1 ];
			byte hitColorB = pixels[ ((texWidth*(y)+x)*4) +2 ];
			byte hitColorA = pixels[ ((texWidth*(y)+x)*4) +3 ];
			
			if (!canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			// early exit if outside threshold
			//if (CompareThreshold(paintColor.r,hitColorR) && CompareThreshold(paintColor.g,hitColorG) && CompareThreshold(paintColor.b,hitColorB) && CompareThreshold(paintColor.a,hitColorA)) return;
			if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA) return;
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			lockMaskPixels = new byte[texWidth * texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (texWidth*(ptsy-1)+ptsx)*4; // down
					if (lockMaskPixels[pixel]==0
					    && CompareThreshold(pixels[pixel+0],hitColorR) 
					    && CompareThreshold(pixels[pixel+1],hitColorG) 
					    && CompareThreshold(pixels[pixel+2],hitColorB) 
					    && CompareThreshold(pixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						DrawPoint(pixel);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<texWidth)
				{
					pixel = (texWidth*ptsy+ptsx+1)*4; // right
					if (lockMaskPixels[pixel]==0
					    && CompareThreshold(pixels[pixel+0],hitColorR) 
					    && CompareThreshold(pixels[pixel+1],hitColorG) 
					    && CompareThreshold(pixels[pixel+2],hitColorB) 
					    && CompareThreshold(pixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						DrawPoint(pixel);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (texWidth*ptsy+ptsx-1)*4; // left
					if (lockMaskPixels[pixel]==0
					    && CompareThreshold(pixels[pixel+0],hitColorR) 
					    && CompareThreshold(pixels[pixel+1],hitColorG) 
					    && CompareThreshold(pixels[pixel+2],hitColorB) 
					    && CompareThreshold(pixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						DrawPoint(pixel);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsy+1<texHeight)
				{
					pixel = (texWidth*(ptsy+1)+ptsx)*4; // up
					if (lockMaskPixels[pixel]==0
					    && CompareThreshold(pixels[pixel+0],hitColorR) 
					    && CompareThreshold(pixels[pixel+1],hitColorG) 
					    && CompareThreshold(pixels[pixel+2],hitColorB) 
					    && CompareThreshold(pixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						DrawPoint(pixel);
						lockMaskPixels[pixel] = 1;
					}
				}
			}
		} // floodfillWithTreshold
		
		/// <summary>
		/// Locks area Floodfill.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		void LockAreaFill(int x,int y)
		{
			Debug.Log ("LockAreaFill");
			byte hitColorR = pixels[ ((texWidth*(y)+x)*4) +0 ];
			byte hitColorG = pixels[ ((texWidth*(y)+x)*4) +1 ];
			byte hitColorB = pixels[ ((texWidth*(y)+x)*4) +2 ];
			byte hitColorA = pixels[ ((texWidth*(y)+x)*4) +3 ];
			
			if (!canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			lockMaskPixels = new byte[texWidth * texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (texWidth*(ptsy-1)+ptsx)*4; // down
					
					if (lockMaskPixels[pixel]==0
					    && (pixels[pixel+0]==hitColorR || pixels[pixel+0]==paintColor.r) 
					    && (pixels[pixel+1]==hitColorG || pixels[pixel+1]==paintColor.g) 
					    && (pixels[pixel+2]==hitColorB || pixels[pixel+2]==paintColor.b) 
					    && (pixels[pixel+3]==hitColorA || pixels[pixel+3]==paintColor.a))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<texWidth)
				{
					pixel = (texWidth*ptsy+ptsx+1)*4; // right
					if (lockMaskPixels[pixel]==0 
					    && (pixels[pixel+0]==hitColorR || pixels[pixel+0]==paintColor.r) 
					    && (pixels[pixel+1]==hitColorG || pixels[pixel+1]==paintColor.g) 
					    && (pixels[pixel+2]==hitColorB || pixels[pixel+2]==paintColor.b) 
					    && (pixels[pixel+3]==hitColorA || pixels[pixel+3]==paintColor.a))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (texWidth*ptsy+ptsx-1)*4; // left
					if (lockMaskPixels[pixel]==0 
					    && (pixels[pixel+0]==hitColorR || pixels[pixel+0]==paintColor.r) 
					    && (pixels[pixel+1]==hitColorG || pixels[pixel+1]==paintColor.g) 
					    && (pixels[pixel+2]==hitColorB || pixels[pixel+2]==paintColor.b) 
					    && (pixels[pixel+3]==hitColorA || pixels[pixel+3]==paintColor.a))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsy+1<texHeight)
				{
					pixel = (texWidth*(ptsy+1)+ptsx)*4; // up
					if (lockMaskPixels[pixel]==0 
					    && (pixels[pixel+0]==hitColorR || pixels[pixel+0]==paintColor.r) 
					    && (pixels[pixel+1]==hitColorG || pixels[pixel+1]==paintColor.g) 
					    && (pixels[pixel+2]==hitColorB || pixels[pixel+2]==paintColor.b) 
					    && (pixels[pixel+3]==hitColorA || pixels[pixel+3]==paintColor.a))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						lockMaskPixels[pixel] = 1;
					}
				}
			}
		} // LockAreaFill
		
		/// <summary>
		/// Floodfill by using mask lock area.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		void LockAreaFillMaskOnly(int x,int y)
		{
			Debug.Log ("LockAreaFillMaskOnly");
			byte hitColorR = maskPixels[ ((texWidth*(y)+x)*4) +0 ];
			byte hitColorG = maskPixels[ ((texWidth*(y)+x)*4) +1 ];
			byte hitColorB = maskPixels[ ((texWidth*(y)+x)*4) +2 ];
			byte hitColorA = maskPixels[ ((texWidth*(y)+x)*4) +3 ];
			
			if (!canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			lockMaskPixels = new byte[texWidth * texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (texWidth*(ptsy-1)+ptsx)*4; // down
					
					if (lockMaskPixels[pixel]==0
					    && (maskPixels[pixel+0]==hitColorR || maskPixels[pixel+0]==paintColor.r) 
					    && (maskPixels[pixel+1]==hitColorG || maskPixels[pixel+1]==paintColor.g) 
					    && (maskPixels[pixel+2]==hitColorB || maskPixels[pixel+2]==paintColor.b) 
					    && (maskPixels[pixel+3]==hitColorA || maskPixels[pixel+3]==paintColor.a))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<texWidth)
				{
					pixel = (texWidth*ptsy+ptsx+1)*4; // right
					if (lockMaskPixels[pixel]==0 
					    && (maskPixels[pixel+0]==hitColorR || maskPixels[pixel+0]==paintColor.r) 
					    && (maskPixels[pixel+1]==hitColorG || maskPixels[pixel+1]==paintColor.g) 
					    && (maskPixels[pixel+2]==hitColorB || maskPixels[pixel+2]==paintColor.b) 
					    && (maskPixels[pixel+3]==hitColorA || maskPixels[pixel+3]==paintColor.a))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (texWidth*ptsy+ptsx-1)*4; // left
					if (lockMaskPixels[pixel]==0 
					    && (maskPixels[pixel+0]==hitColorR || maskPixels[pixel+0]==paintColor.r) 
					    && (maskPixels[pixel+1]==hitColorG || maskPixels[pixel+1]==paintColor.g) 
					    && (maskPixels[pixel+2]==hitColorB || maskPixels[pixel+2]==paintColor.b) 
					    && (maskPixels[pixel+3]==hitColorA || maskPixels[pixel+3]==paintColor.a))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsy+1<texHeight)
				{
					pixel = (texWidth*(ptsy+1)+ptsx)*4; // up
					if (lockMaskPixels[pixel]==0 
					    && (maskPixels[pixel+0]==hitColorR || maskPixels[pixel+0]==paintColor.r) 
					    && (maskPixels[pixel+1]==hitColorG || maskPixels[pixel+1]==paintColor.g) 
					    && (maskPixels[pixel+2]==hitColorB || maskPixels[pixel+2]==paintColor.b) 
					    && (maskPixels[pixel+3]==hitColorA || maskPixels[pixel+3]==paintColor.a))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						lockMaskPixels[pixel] = 1;
					}
				}
			}
		} // LockAreaFillMaskOnly
		
		
		// compares if two values are below threshold
		bool CompareThreshold(byte a, byte b)
		{
			//return Mathf.Abs(a-b)<=threshold;
			if (a<b) {a ^= b; b ^= a; a ^= b;} // http://lab.polygonal.de/?p=81
			return (a-b)<=paintThreshold;
		}
		// create locking mask floodfill, using threshold, checking pixels from mask only
		public void LockAreaFillWithThresholdMaskOnly(int x,int y)
		{
			Debug.Log("LockAreaFillWithThresholdMaskOnly");
			//if (drawMode == DrawMode.Sticker) {
			//System.Array.Copy (undoPixels, pixels, undoPixels.Length);
			//ClearImage();
			//		}
			// get canvas color from this point
			byte hitColorR = maskPixels[ ((texWidth*(y)+x)*4) +0 ];
			byte hitColorG = maskPixels[ ((texWidth*(y)+x)*4) +1 ];
			byte hitColorB = maskPixels[ ((texWidth*(y)+x)*4) +2 ];
			byte hitColorA = maskPixels[ ((texWidth*(y)+x)*4) +3 ];
			
			if (!canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) {/*Debug.Log ("CANT DRAW ON BLACK");*/return;}
			}
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			lockMaskPixels = new byte[texWidth * texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (texWidth*(ptsy-1)+ptsx)*4; // down
					
					if (lockMaskPixels[pixel]==0 // this pixel is not used yet
					    && (CompareThreshold(maskPixels[pixel+0],hitColorR)) // if pixel is same as hit color OR same as paint color
					    && (CompareThreshold(maskPixels[pixel+1],hitColorG)) 
					    && (CompareThreshold(maskPixels[pixel+2],hitColorB)) 
					    && (CompareThreshold(maskPixels[pixel+3],hitColorA)))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<texWidth)
				{
					pixel = (texWidth*ptsy+ptsx+1)*4; // right
					if (lockMaskPixels[pixel]==0 
					    && (CompareThreshold(maskPixels[pixel+0],hitColorR)) // if pixel is same as hit color OR same as paint color
					    && (CompareThreshold(maskPixels[pixel+1],hitColorG)) 
					    && (CompareThreshold(maskPixels[pixel+2],hitColorB)) 
					    && (CompareThreshold(maskPixels[pixel+3],hitColorA)))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (texWidth*ptsy+ptsx-1)*4; // left
					if (lockMaskPixels[pixel]==0 
					    && (CompareThreshold(maskPixels[pixel+0],hitColorR)) // if pixel is same as hit color OR same as paint color
					    && (CompareThreshold(maskPixels[pixel+1],hitColorG)) 
					    && (CompareThreshold(maskPixels[pixel+2],hitColorB)) 
					    && (CompareThreshold(maskPixels[pixel+3],hitColorA)))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						lockMaskPixels[pixel] = 1; 
					}
				}
				
				if (ptsy+1<texHeight)
				{
					pixel = (texWidth*(ptsy+1)+ptsx)*4; // up
					if (lockMaskPixels[pixel]==0 
					    && (CompareThreshold(maskPixels[pixel+0],hitColorR)) // if pixel is same as hit color OR same as paint color
					    && (CompareThreshold(maskPixels[pixel+1],hitColorG)) 
					    && (CompareThreshold(maskPixels[pixel+2],hitColorB))
					    && (CompareThreshold(maskPixels[pixel+3],hitColorA)))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						lockMaskPixels[pixel] = 1; 
					}
				}
			}
		} // LockMaskFillWithTreshold
		
		
		
		// create locking mask floodfill, using threshold
		void LockMaskFillWithThreshold(int x,int y)
		{
			Debug.Log("LockMaskFillWithTreshold");
			// get canvas color from this point
			byte hitColorR = pixels[ ((texWidth*(y)+x)*4) +0 ];
			byte hitColorG = pixels[ ((texWidth*(y)+x)*4) +1 ];
			byte hitColorB = pixels[ ((texWidth*(y)+x)*4) +2 ];
			byte hitColorA = pixels[ ((texWidth*(y)+x)*4) +3 ];
			
			if (!canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			lockMaskPixels = new byte[texWidth * texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (texWidth*(ptsy-1)+ptsx)*4; // down
					
					if (lockMaskPixels[pixel]==0 // this pixel is not used yet
					    && (CompareThreshold(pixels[pixel+0],hitColorR) || CompareThreshold(pixels[pixel+0],paintColor.r)) // if pixel is same as hit color OR same as paint color
					    && (CompareThreshold(pixels[pixel+1],hitColorG) || CompareThreshold(pixels[pixel+1],paintColor.g)) 
					    && (CompareThreshold(pixels[pixel+2],hitColorB) || CompareThreshold(pixels[pixel+2],paintColor.b)) 
					    && (CompareThreshold(pixels[pixel+3],hitColorA) || CompareThreshold(pixels[pixel+3],paintColor.a)))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<texWidth)
				{
					pixel = (texWidth*ptsy+ptsx+1)*4; // right
					if (lockMaskPixels[pixel]==0 
					    && (CompareThreshold(pixels[pixel+0],hitColorR) || CompareThreshold(pixels[pixel+0],paintColor.r)) // if pixel is same as hit color OR same as paint color
					    && (CompareThreshold(pixels[pixel+1],hitColorG) || CompareThreshold(pixels[pixel+1],paintColor.g)) 
					    && (CompareThreshold(pixels[pixel+2],hitColorB) || CompareThreshold(pixels[pixel+2],paintColor.b)) 
					    && (CompareThreshold(pixels[pixel+3],hitColorA) || CompareThreshold(pixels[pixel+3],paintColor.a)))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (texWidth*ptsy+ptsx-1)*4; // left
					if (lockMaskPixels[pixel]==0 
					    && (CompareThreshold(pixels[pixel+0],hitColorR) || CompareThreshold(pixels[pixel+0],paintColor.r)) // if pixel is same as hit color OR same as paint color
					    && (CompareThreshold(pixels[pixel+1],hitColorG) || CompareThreshold(pixels[pixel+1],paintColor.g)) 
					    && (CompareThreshold(pixels[pixel+2],hitColorB) || CompareThreshold(pixels[pixel+2],paintColor.b)) 
					    && (CompareThreshold(pixels[pixel+3],hitColorA) || CompareThreshold(pixels[pixel+3],paintColor.a)))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						lockMaskPixels[pixel] = 1; 
					}
				}
				
				if (ptsy+1<texHeight)
				{
					pixel = (texWidth*(ptsy+1)+ptsx)*4; // up
					if (lockMaskPixels[pixel]==0 
					    && (CompareThreshold(pixels[pixel+0],hitColorR) || CompareThreshold(pixels[pixel+0],paintColor.r)) // if pixel is same as hit color OR same as paint color
					    && (CompareThreshold(pixels[pixel+1],hitColorG) || CompareThreshold(pixels[pixel+1],paintColor.g)) 
					    && (CompareThreshold(pixels[pixel+2],hitColorB) || CompareThreshold(pixels[pixel+2],paintColor.b)) 
					    && (CompareThreshold(pixels[pixel+3],hitColorA) || CompareThreshold(pixels[pixel+3],paintColor.a)))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						lockMaskPixels[pixel] = 1; 
					}
				}
			}
		} // LockMaskFillWithTreshold
		#endregion FLOODFILL
		

		/// <summary>
		/// Reads the current custom brush.This needs to be called if custom brush is changed.
		/// </summary>
		public void ReadCurrentCustomBrush()
		{
			if(customBrush==null) return ;
			
			customBrushWidth=customBrush/*[selectedBrush]*/.width;
			customBrushHeight=customBrush/*[selectedBrush]*/.height;
			customBrushBytes = new byte[customBrushWidth * customBrushHeight * 4];
			Color[] tmp =customBrush.GetPixels();
			int pixel = 0;
			for (int y = 0; y < customBrushHeight; y++) 
			{
				for (int x = 0; x < customBrushWidth; x++) 
				{
					// TODO: take colors from GetPixels
					Color brushPixel = tmp[y*customBrushHeight+x];//customBrush/*[selectedBrush]*/.GetPixel(x,y);//original
					customBrushBytes[pixel] = (byte)(brushPixel.r*255);
					customBrushBytes[pixel+1] = (byte)(brushPixel.g*255);
					customBrushBytes[pixel+2] = (byte)(brushPixel.b*255);
					customBrushBytes[pixel+3] = (byte)(brushPixel.a*255);
					//}
					pixel += 4;
				}
			}
			//precalculate values
			customBrushWidthHalf = (int)(customBrushWidth/2);
			texWidthMinusCustomBrushWidth = texWidth-customBrushWidth;
			texHeightMinusCustomBrushHeight = texHeight-customBrushHeight;
		}
		
		
		
		
		/// <summary>
		/// Reads the current custom pattern.reads current texture pattern into pixel array.
		/// </summary>
		/// <param name="patternTexture">Pattern texture.</param>
		 public void ReadCurrentCustomPattern(Texture2D patternTexture)
		{
			if (patternTexture==null) {/*Debug.LogError("Problem: No custom patterns assigned on "+gameObject.name);*/ return;}
			this.pattenTexture=patternTexture;
			customPatternWidth=patternTexture.width;
			customPatternHeight=patternTexture.height;
			patternBrushBytes = new byte[customPatternWidth * customPatternHeight * 4];
			Color[] tmp =patternTexture.GetPixels();
			int pixel = 0;
			for (int x = 0; x < customPatternWidth; x++)
			{
				for (int y = 0; y < customPatternHeight; y++)
				{
					Color brushPixel = tmp[y*customPatternHeight+x];//patternTexture.GetPixel(x,y);
					
					patternBrushBytes[pixel] = (byte)(brushPixel.r*255);
					patternBrushBytes[pixel+1] = (byte)(brushPixel.g*255);
					patternBrushBytes[pixel+2] = (byte)(brushPixel.b*255);
					patternBrushBytes[pixel+3] = (byte)(brushPixel.a*255);
					
					pixel += 4;
				}
			}
		}
		
		// draws single point to this pixel coordinate, with current paint color
		public void DrawPoint(int x,int y)
		{
			int pixel = (texWidth*y+x)*4;
			if(brushMode!=BrushProperties.Pattern)
			{
			pixels[pixel] = paintColor.r;
			pixels[pixel+1] = paintColor.g;
			pixels[pixel+2] = paintColor.b;
			pixels[pixel+3] = paintColor.a;
			}
			//FIXME!
			
			else
			{
				float yy = Mathf.Repeat(y,customPatternWidth);
				float xx = Mathf.Repeat(x,customPatternWidth);
				int pixel2 = (int) Mathf.Repeat( (customPatternWidth*xx+yy)*4, patternBrushBytes.Length);
				pixels[pixel] =patternBrushBytes[pixel2];
				pixels[pixel+1] = patternBrushBytes[pixel2+1];
				pixels[pixel+2] = patternBrushBytes[pixel2+2];
				pixels[pixel+3] = patternBrushBytes[pixel2+3];
			}
		}
		
		
		// draws single point to this pixel array index, with current paint color
		public void DrawPoint(int pixel)
		{
			pixels[pixel] = paintColor.r;
			pixels[pixel+1] = paintColor.g;
			pixels[pixel+2] = paintColor.b;
			pixels[pixel+3] = paintColor.a;
		}
		// draw line between 2 points (if moved too far/fast)
		// http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		public void DrawLine(Vector2 start, Vector2 end)
		{
			int x0=(int)start.x;
			int y0=(int)start.y;
			int x1=(int)end.x;
			int y1=(int)end.y;
			int dx= Mathf.Abs(x1-x0); // TODO: try these? http://stackoverflow.com/questions/6114099/fast-integer-abs-function
			int dy= Mathf.Abs(y1-y0);
			int sx,sy;
			if (x0 < x1) {sx=1;}else{sx=-1;}
			if (y0 < y1) {sy=1;}else{sy=-1;}
			int err=dx-dy;
			bool loop=true;
			//			int minDistance=brushSize-1;
			int minDistance=(int)(brushSize>>1); // divide by 2, you might want to set mindistance to smaller value, to avoid gaps between brushes when moving fast
			int pixelCount=0;
			int e2;
			while (loop) 
			{
				pixelCount++;
				if (pixelCount>minDistance)
				{
					pixelCount=0;
					DrawCircle(x0,y0);
				}
				if ((x0 == x1) && (y0 == y1)) loop=false;
				e2 = 2*err;
				if (e2 > -dy)
				{
					err = err - dy;
					x0 = x0 + sx;
				}
				if (e2 <  dx)
				{
					err = err + dx;
					y0 = y0 + sy;
				}
			}
		} // drawline
		
		void DrawLineWithBrush(Vector2 start, Vector2 end)
		{
			int x0=(int)start.x;
			int y0=(int)start.y;
			int x1=(int)end.x;
			int y1=(int)end.y;
			int dx= Mathf.Abs(x1-x0); // TODO: try these? http://stackoverflow.com/questions/6114099/fast-integer-abs-function
			int dy= Mathf.Abs(y1-y0);
			int sx,sy;
			if (x0 < x1) {sx=1;}else{sx=-1;}
			if (y0 < y1) {sy=1;}else{sy=-1;}
			int err=dx-dy;
			bool loop=true;
			//			int minDistance=brushSize-1;
			int minDistance=(int)(brushSize>>1); // divide by 2, you might want to set mindistance to smaller value, to avoid gaps between brushes when moving fast
			int pixelCount=0;
			int e2;
			while (loop) 
			{
				pixelCount++;
				if (pixelCount>minDistance)
				{
					pixelCount=0;
					DrawCustomBrush2(x0,y0);
				}
				if ((x0 == x1) && (y0 == y1)) loop=false;
				e2 = 2*err;
				if (e2 > -dy)
				{
					err = err - dy;
					x0 = x0 + sx;
				}
				if (e2 <  dx)
				{
					err = err + dx;
					y0 = y0 + sy;
				}
			}
		}
		
		
		void DrawLineWithPattern(Vector2 start, Vector2 end)
		{
			int x0=(int)start.x;
			int y0=(int)start.y;
			int x1=(int)end.x;
			int y1=(int)end.y;
			int dx= Mathf.Abs(x1-x0); // TODO: try these? http://stackoverflow.com/questions/6114099/fast-integer-abs-function
			int dy= Mathf.Abs(y1-y0);
			int sx,sy;
			if (x0 < x1) {sx=1;}else{sx=-1;}
			if (y0 < y1) {sy=1;}else{sy=-1;}
			int err=dx-dy;
			bool loop=true;
			//			int minDistance=brushSize-1;
			int minDistance=(int)(brushSize>>1); // divide by 2, you might want to set mindistance to smaller value, to avoid gaps between brushes when moving fast
			int pixelCount=0;
			int e2;
			while (loop) 
			{
				pixelCount++;
				if (pixelCount>minDistance)
				{
					pixelCount=0;
					DrawPatternCircle(x0,y0);
				}
				if ((x0 == x1) && (y0 == y1)) loop=false;
				e2 = 2*err;
				if (e2 > -dy)
				{
					err = err - dy;
					x0 = x0 + sx;
				}
				if (e2 <  dx)
				{
					err = err + dx;
					y0 = y0 + sy;
				}
			}
		}
		
		// init/clear image, this can be called outside this script also
		public void ClearImage()
		{
			if (usingClearingImage)
			{
				ClearImageWithImage();
			}
			
			
			else{
				
				int pixel = 0;
				for (int y = 0; y < texHeight; y++) 
				{
					for (int x = 0; x < texWidth; x++) 
					{
						pixels[pixel] = clearColor.r;
						pixels[pixel+1] = clearColor.g;
						pixels[pixel+2] = clearColor.b;
						pixels[pixel+3] = clearColor.a;
						pixel += 4;
						
					}
				}
				tex.LoadRawTextureData(pixels);
				tex.Apply(true);
				
			}
		} // clear image
	
		public void ClearImageWithImage()
		{
			// fill pixels array with clearpixels array
			System.Array.Copy(clearPixels,0,pixels,0,clearPixels.Length);
			
			
			// just assign our clear image array into tex
			tex.LoadRawTextureData(clearPixels);
			tex.Apply(false);
		} // clear image
		
		
		public void ReadMaskImage()
		{
			maskPixels = new byte[texWidth * texHeight * 4];
			
			int pixel = 0;
			for (int y = 0; y < texHeight; y++) 
			{
				for (int x = 0; x < texWidth; x++) 
				{
					Color c = maskTex.GetPixel(x,y);	
					maskPixels[pixel] = (byte)(c.r*255);
					maskPixels[pixel+1] = (byte)(c.g*255);
					maskPixels[pixel+2] = (byte)(c.b*255);
					maskPixels[pixel+3] = (byte)(c.a*255);
					pixel += 4;
				}
			}
			
		}
		
		public void ReadClearingImage()
		{
			clearPixels = new byte[texWidth * texHeight * 4];
			
			// get our current texture into tex
			tex.SetPixels32(((Texture2D)GetComponent<Renderer>().material.GetTexture(targetTexture)).GetPixels32());
			tex.Apply(false);
			
			int pixel = 0;
			for (int y = 0; y < texHeight; y++) 
			{
				for (int x = 0; x < texWidth; x++) 
				{
					// TODO: use readpixels32
					Color c = tex.GetPixel(x,y);
					
					clearPixels[pixel] = (byte)(c.r*255);
					clearPixels[pixel+1] = (byte)(c.g*255);
					clearPixels[pixel+2] = (byte)(c.b*255);
					clearPixels[pixel+3] = (byte)(c.a*255);
					pixel += 4;
				}
			}
		}
		/// <summary>
		/// Creates the canvas based quad, and mesh collider, scales mesh to fit actial rect transform size of this object.
		/// </summary>  
		void CreateCanvasQuad()
		{
//			// create mesh plane
			Mesh go_Mesh = GetComponent<MeshFilter>().mesh;
			//clear mesh data
			go_Mesh.Clear();
			//come arrays we will use
			Vector3 [] corners= new Vector3[4];
			Vector3 [] corners1= new Vector3[4];
			//get actual object scale based on based hiearchy up to root node
			Vector3 canvasScale=transform.localScale;
			Transform up=transform.parent;
			do{
				canvasScale.x*=up.localScale.x;
				canvasScale.y*=up.localScale.y;
				canvasScale.z*=up.localScale.z;
				up=up.parent;
			}while(up!=null);
			//tace current position in world space
			Vector3 canvasPosition=transform.position;
			//Debug.Log ("CANVAS POSITION "+ transform.position.ToString() + " , "+ gameObject.GetComponent<RectTransform>().anchoredPosition.ToString());
			//get 
			//Rect r= gameObject.GetComponent<RectTransform>().rect;
			//translate object to origin
			transform.position=Vector3.zero;
			//calculate scale factor
			canvasScale.x=1f/canvasScale.x; 
			canvasScale.y=1f/canvasScale.y;
			canvasScale.z=1f/canvasScale.z;
			//get actual world corners of RectTransform 
			gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
//			Debug.Log (corners[0]);
//			Debug.Log (corners[1]);
//			Debug.Log (corners[2]);
//			Debug.Log (corners[3]);
			//up/down scale each world corner 
			for(int i=0;i<4;i++)
			{
			Vector3 newC=corners[i];
			newC.x*=(canvasScale.x);
			newC.y*=(canvasScale.y);
			newC.z*=(canvasScale.z);
			corners1[i]=newC;
			}
			//return object from origin to actual position
			transform.position=canvasPosition;
			//Debug.Log ("CANVAS POSITION END"+ transform.position.ToString() + " , "+ gameObject.GetComponent<RectTransform>().anchoredPosition.ToString());
			//assign mesh vertices
			go_Mesh.vertices = new [] {
			 // bottom left
				corners1[0],
			 // top left
				corners1[1],
			 // top right
				corners1[2],
			 // bottom right
				corners1[3]
			};
			//generate quad UV's
			go_Mesh.uv = new [] {new Vector2(0, 0), new Vector2(0, 1),new Vector2(1, 1), new Vector2(1, 0)};
			//quad gets made by using two triangles ( look some OpenGL basics for this)
			go_Mesh.triangles = new  [] {0, 1, 2, 0, 2, 3};
			//recalcualte mesh normals
			// TODO: add option for this
			go_Mesh.RecalculateNormals();
			//calculate mesh tangents
			// TODO: add option to calculate tangents
			go_Mesh.tangents = new [] {new Vector4(1.0f, 0.0f, 0.0f, -1.0f),new Vector4(1.0f, 0.0f, 0.0f, -1.0f),new Vector4(1.0f, 0.0f, 0.0f, -1.0f),new Vector4(1.0f, 0.0f, 0.0f, -1.0f)};

            // add mesh collider
            //gameObject.AddComponent<UnityEngine.MeshCollider>();
			GetComponent<MeshCollider>().sharedMesh = go_Mesh;
		}
		
		
//		public void SetBrushSize(int newSize)
//		{
//			// no validation is done, should be always bigger than 1
//			brushSize = newSize;
//		}
		
//		public void SetDrawModeBrush()
//		{
//			drawMode = DrawMode.Default;
//		}
//		
//		public void SetDrawModeFill()
//		{
//			drawMode = DrawMode.FloodFill;
//		}
//		
//		public void SetDrawModeShapes()
//		{
//			drawMode = DrawMode.CustomBrush;
//		}
//		
//		public void SetDrawModePattern()
//		{
//			drawMode = DrawMode.Pattern;
//		}
		
//		// returns current image (later: including all layers) as Texture2D
//		public Texture2D GetCanvasAsTexture()
//		{
//			var image = new Texture2D((int)(texWidth/resolutionScaler), (int)(texHeight/resolutionScaler), TextureFormat.RGBA32, false);
//			
//			// TODO: combine layers to single texture
//			
//			image.LoadRawTextureData(pixels);
//			image.Apply(false);
//			return image;
//		}
		
		
		/// <summary>
		/// Draws the custom brush (v2).
		/// </summary>
		/// <param name="px">Px.</param>
		/// <param name="py">Py.</param>
		public void DrawCustomBrush2(int px,int py)
		{
			// TODO: this function needs comments/info..
			Debug.Log ("DrawCustomBrush2");
			// get position where we paint
			int startX=/*(int)*/(px-customBrushWidthHalf);
			int startY=/*(int)*/(py-customBrushWidthHalf);
			int pixel = (texWidth*startY+startX)*4;
			int brushPixel = 0;
			bool skip=false;
			float yy =0;
			float xx =0;
			int pixel2 = 0;
			for (int y = 0; y < customBrushHeight; y++) 
			{
				for (int x = 0; x < customBrushWidth; x++) 
				{
					//brushColor = (customBrushPixels[x*customBrushWidth+y]);
					//FIX
					brushPixel = (customBrushWidth*(y)+x)*4;
					skip=false;
					if((startX+x)>(texWidth-2) || (startX+x)<-1 ) skip=true;
					//if((startY+y)>(texWidth+2) || (startY+y)<-1 ) skip=true;
					if(pixel<0|| pixel>=pixels.Length)
						skip=true;//
					if(brushPixel<0 || brushPixel>customBrushBytes.Length) skip=true;
					if(!canDrawOnBlack && !skip)
					{
						if(pixels[pixel+3]!=0 && pixels[pixel]==0 && pixels[pixel+1]==0 && pixels[pixel+2]==0) skip=true;
					}
					// brush alpha is over 0 in this pixel?
					if ( customBrushBytes[brushPixel+3]!=0 && !skip)
					//END FIX
					{
						
						// take alpha from brush?
						if (useCustomBrushAlpha)
						{
							if (useAdditiveColors)
							{
								
								// additive over white also
								if((useLockArea && lockMaskPixels[pixel]==1) || !useLockArea){//this enables custom brushes using lock mask
									
									switch(brushMode){
									case BrushProperties.Clear:
									//TODO
										pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel],clearColor.r,customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],clearColor.g,customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],clearColor.b,customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],clearColor.a,customBrushBytes[brushPixel+3]/255f);
									break;
									case BrushProperties.Default:
									//TODO
										pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel],paintColor.r,customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],paintColor.g,customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],paintColor.b,customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],paintColor.a,customBrushBytes[brushPixel+3]/255f);
										break;
									case BrushProperties.Simple:
										pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel],customBrushBytes[brushPixel],customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],customBrushBytes[brushPixel+1],customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],customBrushBytes[brushPixel+2],customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],customBrushBytes[brushPixel+3],customBrushBytes[brushPixel+3]/255f);
										break;
									case BrushProperties.Pattern:
									//TODO
										/*float*/ yy = Mathf.Repeat(/*py+y*/py+(y-customBrushHeight/2f),customPatternHeight);
										/*float*/ xx = Mathf.Repeat(/*px+x*/px+(x-customBrushWidth/2f),customPatternWidth);
										/*int*/ pixel2 = (int) Mathf.Repeat( (customPatternWidth*xx +yy)*4, patternBrushBytes.Length);
										pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel],patternBrushBytes[pixel2],customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],patternBrushBytes[pixel2+1],customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],patternBrushBytes[pixel2+2],customBrushBytes[brushPixel+3]/255f);
										pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],patternBrushBytes[pixel2+3],customBrushBytes[brushPixel+3]/255f);
										break;
									
									}
								}
								
							}else{ 
								//TODO
								// no additive colors
								if((useLockArea && lockMaskPixels[pixel]==1) || !useLockArea){
									switch (brushMode){
									case BrushProperties.Clear :
										pixels[pixel] =clearColor.r;
										pixels[pixel+1] = clearColor.g;
										pixels[pixel+2] = clearColor.b;
										pixels[pixel+3] = clearColor.a;
										break;
									case BrushProperties.Default:
										pixels[pixel] =paintColor.r;
										pixels[pixel+1] = paintColor.g;
										pixels[pixel+2] = paintColor.b;
										pixels[pixel+3] = paintColor.a;
										break;
									case BrushProperties.Simple:
										pixels[pixel] =customBrushBytes[brushPixel];
										pixels[pixel+1] = customBrushBytes[brushPixel+1];
										pixels[pixel+2] = customBrushBytes[brushPixel+2];
										pixels[pixel+3] = customBrushBytes[brushPixel+3];
										break;
									case BrushProperties.Pattern:
										/*float*/ yy = Mathf.Repeat(/*py+y*/py+(y-customBrushHeight/2f),customPatternHeight);
										/*float*/ xx = Mathf.Repeat(/*px+x*/px+(x-customBrushWidth/2f),customPatternWidth);
										/*int*/ pixel2 = (int) Mathf.Repeat( (customPatternWidth*xx+yy)*4, patternBrushBytes.Length);
										
										//OVDE TODO
										//add ignore transparent pixels
										pixels[pixel] =patternBrushBytes[pixel2];
										pixels[pixel+1] = patternBrushBytes[pixel2+1];
										pixels[pixel+2] = patternBrushBytes[pixel2+2];
										pixels[pixel+3] = patternBrushBytes[pixel2+3];
										break;
									}
								}
							}
							
						}else{ // use paint color alpha
							
							if (useAdditiveColors)
							{
								if((useLockArea && lockMaskPixels[pixel]==1) || !useLockArea){
									switch (brushMode)
									{
									case BrushProperties.Clear:
										pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel],clearColor.r,paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],clearColor.g,paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],clearColor.b,paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],clearColor.a,paintColor.a/255f*brushAlphaStrength);
										break;
									case BrushProperties.Default:
										pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel],paintColor.r,paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],paintColor.g,paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],paintColor.b,paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],paintColor.a,paintColor.a/255f*brushAlphaStrength);
										break;
									case BrushProperties.Simple:
										pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel],customBrushBytes[brushPixel],paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],customBrushBytes[brushPixel+1],paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],customBrushBytes[brushPixel+2],paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],customBrushBytes[brushPixel+3],paintColor.a/255f*brushAlphaStrength);
										break;
									case BrushProperties.Pattern:
										/*float*/ yy = Mathf.Repeat(/*py+y*/py+(y-customBrushHeight/2f),customPatternHeight);
										/*float*/ xx = Mathf.Repeat(/*px+x*/px+(x-customBrushWidth/2f),customPatternWidth);
										/*int*/ pixel2 = (int) Mathf.Repeat( (customPatternWidth*xx+yy)*4, patternBrushBytes.Length);
										pixels[pixel] = (byte)Mathf.Lerp(pixels[pixel],patternBrushBytes[pixel2],paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+1] = (byte)Mathf.Lerp(pixels[pixel+1],patternBrushBytes[pixel2+1],paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+2] = (byte)Mathf.Lerp(pixels[pixel+2],patternBrushBytes[pixel2+2],paintColor.a/255f*brushAlphaStrength);
										pixels[pixel+3] = (byte)Mathf.Lerp(pixels[pixel+3],patternBrushBytes[pixel2+3],paintColor.a/255f*brushAlphaStrength);
										break;
										
										
									}
								}
								
							}else{ // no additive colors
								if((useLockArea && lockMaskPixels[pixel]==1) || !useLockArea){ 
									
									switch (brushMode){
									case BrushProperties.Clear :
										pixels[pixel] =clearColor.r;
										pixels[pixel+1] = clearColor.g;
										pixels[pixel+2] = clearColor.b;
										pixels[pixel+3] = clearColor.a;
										break;
									case BrushProperties.Default:
										pixels[pixel] =paintColor.r;
										pixels[pixel+1] = paintColor.g;
										pixels[pixel+2] = paintColor.b;
										pixels[pixel+3] = paintColor.a;
										break;
									case BrushProperties.Simple:
										pixels[pixel] =customBrushBytes[brushPixel];
										pixels[pixel+1] = customBrushBytes[brushPixel+1];
										pixels[pixel+2] = customBrushBytes[brushPixel+2];
										pixels[pixel+3] = customBrushBytes[brushPixel+3];
										break;
									case BrushProperties.Pattern:
										/*float*/ yy = Mathf.Repeat(/*py+y*/py+(y-customBrushHeight/2f),customPatternHeight);
										/*float*/ xx = Mathf.Repeat(/*px+x*/px+(x-customBrushWidth/2f),customPatternWidth);
										/*int*/ pixel2 = (int) Mathf.Repeat( (customPatternWidth*xx+yy)*4, patternBrushBytes.Length);
										pixels[pixel] =patternBrushBytes[pixel2];
										pixels[pixel+1] = patternBrushBytes[pixel2+1];
										pixels[pixel+2] = patternBrushBytes[pixel2+2];
										pixels[pixel+3] = patternBrushBytes[pixel2+3];
										break;
									}
								}
							}
						}
						
					} // if alpha>0
					pixel+= 4;
					
				} // for x
//				pixel = (texWidth*(startY==0?1:startY+y)+startX+1)*4;
				pixel = (texWidth*(startY==0?-1:startY+y)+startX+1)*4;
				//pixel = (texWidth*(startY==0?1:startY+y)+(startX==0?1:startX)+1)*4;
			} // for y
			
		}//end of function : DrawCustomBrush2
		
		
		/// <summary>
		/// Generates the drawing mask based on texture.
		/// Funkcija koja generise odgovarajucu masku na osnovu izvorisne teksture.
		/// </summary>
		/// <param name="source">Source.</param>
		public Texture2D GenerateDrawingMaskBasedOnTexture(Texture2D source)
		{
			Color t = Color.clear;
			Color b = Color.black;
			b.a=1;
			t.a=0;
			Texture2D drawingMask =  Texture2D.blackTexture;//new Texture2D (nailMasks [index].width, nailMasks [index].height, TextureFormat.ARGB32, false, false);
			drawingMask.Reinitialize(source.width, source.height, TextureFormat.ARGB32, false) ;
			drawingMask.filterMode = FilterMode.Bilinear;
			
			for (int i=0; i<source.width; i++)
			for (int j=0; j<source.height; j++) {
				Color c= source.GetPixel(i,j);
				if(c.a>0.5f)
				{
					drawingMask.SetPixel(i,j,t);
				}
				else
					drawingMask.SetPixel(i,j,b);
			}
			drawingMask.Apply (false);
			return drawingMask;		
		}
		/// <summary>
		/// Sets the mask texture mode drawing mode.
		/// </summary>
		public void SetMaskTextureMode()
		{
		
			useMaskImage = true;
			useLockArea = true;
			ReadMaskImage ();
			gameObject.GetComponent<Renderer>().material.SetTexture("_MaskTex", maskTex);
		}
		
		/// <summary>
		/// Setovanje the bitmap brusha.
		/// </summary>
		/// <returns><c>true</c>, if bitmap brush was set, <c>false</c> otherwise.</returns>
		/// <param name="brushTexture">Brush texture.</param>
		/// <param name="brushType">Brush type.</param>
		/// <param name="isAditiveBrush">If set to <c>true</c> is aditive brush.</param>
		/// <param name="brushCanDrawOnBlack">If set to <c>true</c> brush can draw on black.</param>
		/// <param name="brushColor">Brush color.</param>
		/// <param name="usesLockMasks">If set to <c>true</c> uses lock masks.</param>
		/// <param name="useBrushAlpha">If set to <c>true</c> use brush alpha.</param>
		/// <param name="brushPattern">Brush pattern.</param>
		public bool SetBitmapBrush(Texture2D brushTexture,BrushProperties brushType, bool isAditiveBrush,bool brushCanDrawOnBlack,Color brushColor, bool usesLockMasks,bool useBrushAlpha,Texture2D brushPattern )
		{
			customBrush=brushTexture;
			ReadCurrentCustomBrush();
			brushMode=brushType;
			useAdditiveColors=isAditiveBrush;
			canDrawOnBlack=brushCanDrawOnBlack;
			paintColor=brushColor;
			useLockArea=usesLockMasks;
			useMaskLayerOnly=usesLockMasks;
			useThreshold=usesLockMasks;
			useCustomBrushAlpha=useBrushAlpha;
			if(brushPattern!=null)
			{
			ReadCurrentCustomPattern(brushPattern);
			}
			return true;
		}
		/// <summary>
		/// Setovanje the vector brusha.
		/// </summary>
		/// <returns><c>true</c>, if vector brush was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type.</param>
		/// <param name="size">Size.</param>
		/// <param name="brushColor">Brush color.</param>
		/// <param name="pattern">Pattern.</param>
		/// <param name="isAditiveBrush">If set to <c>true</c> is aditive brush.</param>
		/// <param name="brushCanDrawOnBlack">If set to <c>true</c> brush can draw on black.</param>
		/// <param name="usesLockMasks">If set to <c>true</c> uses lock masks.</param>
		/// <param name="useBrushAlpha">If set to <c>true</c> use brush alpha.</param>
		public bool SetVectorBrush(VectorBrush type, int size, Color brushColor, Texture2D pattern,bool isAditiveBrush,bool brushCanDrawOnBlack, bool usesLockMasks,bool useBrushAlpha)
		{
		if(pattern!=null)
		{
			//Debug.Log ("CIRCLE");
			drawMode=DrawMode.Default;
			ReadCurrentCustomPattern(pattern);
			brushMode=BrushProperties.Pattern;
		}
		else
		{
			drawMode=DrawMode.Default;
			paintColor=brushColor;
			brushMode=BrushProperties.Default;
		}
		useAdditiveColors=isAditiveBrush;
		canDrawOnBlack=brushCanDrawOnBlack;
		useCustomBrushAlpha=useBrushAlpha;
		useLockArea=usesLockMasks;
		brushSize=size;
		return true;
		}
		/// <summary>
		/// Setuje  Flood Fill brush (bucket).
		/// </summary>
		/// <returns><c>true</c>, if flood F ill brush was set, <c>false</c> otherwise.</returns>
		/// <param name="floodColor">Flood color.</param>
		/// <param name="usesLockMasks">If set to <c>true</c> uses lock masks.</param>
		public bool SetFloodFIllBrush(Color floodColor,bool usesLockMasks)
		{
		drawMode=DrawMode.FloodFill;
		paintColor=floodColor;
		useLockArea=usesLockMasks;
		return true;
		}
		/// <summary>
		/// Setuje teksturu po kojoj se crta.
		/// </summary>
		/// <returns><c>true</c>, if drawing texture was set, <c>false</c> otherwise.</returns>
		/// <param name="texture">Texture.</param>
		public bool SetDrawingTexture (Texture2D texture)
		{
			Debug.Log(this.gameObject.name);
		Color [] texturePixels=texture.GetPixels();
		pixels= new byte[texture.width*texture.height*4];
		int pix=0;
		for(int i=0;i<texture.height; i++)
			for(int j=0;j<texture.width; j++)
				{
				pixels[pix]=(byte)(texturePixels[i*texture.width+j].r*255);//R
				pixels[pix+1]=(byte)(texturePixels[i*texture.width+j].g*255);//G
				pixels[pix+2]=(byte)(texturePixels[i*texture.width+j].b*255);//B
				pixels[pix+3]=(byte)(texturePixels[i*texture.width+j].a*255);//A
				pix+=4;
				}
		//for(int i=0;i<texture.width;i++)
		//	for(int j=0;j<texture.height;j++)
		//		{
		//		pixels[pix]=(byte)(texturePixels[i*texture.width+j].r*255);//R
		//		pixels[pix+1]=(byte)(texturePixels[i*texture.width+j].g*255);//G
		//		pixels[pix+2]=(byte)(texturePixels[i*texture.width+j].b*255);//B
		//		pixels[pix+3]=(byte)(texturePixels[i*texture.width+j].a*255);//A
		//		pix+=4;
		//		}
		tex.LoadRawTextureData(pixels);
		tex.Apply(false);
		GetComponent<Renderer>().material.SetTexture(targetTexture, tex);
		texWidth=texture.width;
		texHeight=texture.height;	
		if (createCanvasMesh)
			{

				/*Sprite*/ //imageSprite=Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(/*texWidth/2f,tex.height/2f*/0.5f,0.5f));
				//imageSprite.texture.LoadRawTextureData(pixels);
				
				gameObject.GetComponent<MeshRenderer>().enabled=false;
				//gameObject.GetComponent<Image>().sprite=imageSprite;
				//gameObject.GetComponent<Image>().enabled=true;
				gameObject.GetComponent<RawImage>().texture=tex;
				gameObject.GetComponent<RawImage>().enabled=true;
				
			}
		
		if(undoEnabled)
		{
			undoPixels= new byte[pixels.Length];
			System.Array.Copy(pixels,undoPixels,pixels.Length);
		}
		return true;
		}
		/// <summary>
		/// Setuje teksturu maske.
		/// </summary>
		/// <returns><c>true</c>, if drawing mask was set, <c>false</c> otherwise.</returns>
		/// <param name="texture">Texture.</param>
		public bool SetDrawingMask(Texture2D texture)		
		{
			Color [] texturePixels=texture.GetPixels();
			maskPixels= new byte[texture.width*texture.height*4];
			int pix=0;
			for(int i=0;i<texture.height; i++)
				for(int j=0;j<texture.width; j++)
			{
				maskPixels[pix]=(byte)(texturePixels[i*texture.width+j].r*255);//R
				maskPixels[pix+1]=(byte)(texturePixels[i*texture.width+j].g*255);//G
				maskPixels[pix+2]=(byte)(texturePixels[i*texture.width+j].b*255);//B
				maskPixels[pix+3]=(byte)(texturePixels[i*texture.width+j].a*255);//A
				pix+=4;
			}
			//for(int i=0;i<texture.width;i++)
			//	for(int j=0;j<texture.height;j++)
			//{
			//	if(texturePixels[i*texture.width+j].r>0)
			//	maskPixels[pix]=(byte)(texturePixels[i*texture.width+j].r*255);//R
			//	maskPixels[pix+1]=(byte)(texturePixels[i*texture.width+j].g*255);//G
			//	maskPixels[pix+2]=(byte)(texturePixels[i*texture.width+j].b*255);//B
			//	maskPixels[pix+3]=(byte)(texturePixels[i*texture.width+j].a*255);//A
			//	pix+=4;
			//}
		return true;
		}
	}
	

	
}