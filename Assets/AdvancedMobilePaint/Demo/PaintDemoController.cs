using UnityEngine;
using System.Collections;
using AdvancedMobilePaint;

/// <summary>
/// Paint demo controller.
/// </summary>
public class PaintDemoController : MonoBehaviour {
	public AdvancedMobilePaint.AdvancedMobilePaint paintEngine;
	
	public Texture2D paintSurface;
	public Texture2D maskTexture;
	public Texture2D brushExample1;
	public Texture2D brushExample2;
	public Texture2D brushExample3;
	public Texture2D patternExample1;
	public Texture2D patternExample2;
	public Texture2D patternExample3;
	
	public Sprite patternSprite;
	public Sprite patternSprite2;

	
	bool lockMode=false;
	
	public PaintUndoManager undoManager;
	// Use this for initialization
	void Start () {
	
	//SetUpQuadPaint();
	patternExample2=paintEngine.ConvertSpriteToTexture2D(patternSprite2);
	DoReset();
	
	}
	
//	// Update is called once per frame
//	void Update () {
//	

//	}
	public void SetUpQuadPaint()
	{
		paintEngine.SetDrawingTexture(paintSurface);
		paintEngine.useLockArea=false;
		paintEngine.useMaskLayerOnly=false;
		paintEngine.useThreshold=false;
		paintEngine.drawEnabled=true;
		//paintEngine.SetDrawingMask(paintEngine.ConvertSpriteToTexture2D(patternSprite));
	}
	
	public void SetUpBitmapBrushType1()
	{
		paintEngine.drawMode=AdvancedMobilePaint.DrawMode.CustomBrush;
		paintEngine.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Default,false,true,Color.blue,false,false,null);
		paintEngine.drawEnabled=true;
	}
	
	public void SetUpBitmapBrushType2()
	{
		paintEngine.drawMode=AdvancedMobilePaint.DrawMode.CustomBrush;
		paintEngine.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Pattern,false,true,Color.blue,false,false,paintEngine.ConvertSpriteToTexture2D(patternSprite));
		paintEngine.drawEnabled=true;
	}
	
	public void ToogleLockMask()
	{
	lockMode=!lockMode;
	
	if(lockMode)
	{
		paintEngine.SetDrawingMask(maskTexture);
		paintEngine.useLockArea=true;
		paintEngine.useMaskLayerOnly=true;
		paintEngine.useThreshold=true;
	}
	else
	{
			paintEngine.useLockArea=false;
			paintEngine.useMaskLayerOnly=false;
			paintEngine.useThreshold=false;
	}
	
	}
	public void SetUpBitmapBrushType3()
	{
		paintEngine.drawMode=AdvancedMobilePaint.DrawMode.CustomBrush;
		
		paintEngine.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Pattern,false,true,Color.blue,false,false,patternExample2);
		paintEngine.drawEnabled=true;
	}
	public void SetUppFlooodFillBrush()
	{
		paintEngine.SetFloodFIllBrush(Color.green,true);
		paintEngine.useLockArea=false;
		paintEngine.useMaskLayerOnly=false;
		paintEngine.useThreshold=false;
		paintEngine.drawEnabled=true;
	}
	
	public void DoUndo()
	{
		undoManager.UndoLastStep();
	}
	public void DoRedo()
	{
		undoManager.RedoLastStep();
	}
	public void DoReset()
	{
		undoManager.ClearSteps();
		SetUpQuadPaint();
	}
	
	public void SetUpVectorBrushType()
	{
		paintEngine.drawMode=AdvancedMobilePaint.DrawMode.Default;
		paintEngine.SetVectorBrush(VectorBrush.Circle,32,Color.red,null,false,false,false,false);
		paintEngine.drawEnabled=true;
	}
	public void SetUpVectorBrushType2()
	{
		paintEngine.drawMode=AdvancedMobilePaint.DrawMode.Pattern;
		paintEngine.SetVectorBrush(VectorBrush.Circle,32,Color.gray,patternExample1,false,false,false,false);
		paintEngine.drawEnabled=true;
	}
}
