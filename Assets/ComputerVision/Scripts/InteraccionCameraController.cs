using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteraccionCameraController : MonoBehaviour
{
    // UI RawImage we're applying the web cam texture to
    public RawImage cameraProjection;

    // texture which displays what our camera is seeing
    private WebCamTexture camTex;
	public AspectRatioFitter fit;
	
	public GameObject UIPrincipal;
	public GameObject UIPopUp2;
	public GameObject UIPopUp3;
	
	private float timer = 0;
	private float timerMax = 0;

    void Start ()
    {
        // create the camera texture
        camTex = new WebCamTexture(Screen.width, Screen.height);
        camTex.Play();
		cameraProjection.texture = camTex;
    }

    void Update ()
    {
		float ratio = (float)camTex.width / (float)camTex.height;
		fit.aspectRatio=ratio;
		
		float scaleY = camTex.videoVerticallyMirrored ? -1f: 1f;
		cameraProjection.rectTransform.localScale = new Vector3(1f * ratio, scaleY * ratio,  1f * ratio); 
		
		int orient = -camTex.videoRotationAngle;
		cameraProjection.rectTransform.localEulerAngles = new Vector3(0,0,orient);
        // click / touch input to take a picture
		if (UIPrincipal.activeSelf && !UIPopUp2.activeSelf && !UIPopUp3.activeSelf){
			
			if(!Waited(7)) return;
			StartCoroutine(TakePicture());
			timer = 0;
			
			/*if (Input.GetMouseButtonDown(0))
				StartCoroutine(TakePicture());
			else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
				StartCoroutine(TakePicture());	*/
		}
        
    }

    // takes a picture and converts the data to a byte array
    // then triggers the AppManager.GetImageData method
    IEnumerator TakePicture ()
    {
        yield return new WaitForEndOfFrame();

        // create a new texture the size of the web cam texture
        Texture2D screenTex = new Texture2D(camTex.width, camTex.height);

        // read the pixels on the web cam texture and apply them
        screenTex.SetPixels(camTex.GetPixels());
        screenTex.Apply();

        // convert the texture to PNG, then get the data as a byte array
        byte[] byteData = screenTex.EncodeToPNG();

        // send the image data off to the Computer Vision API
        InteraccionAppManager.instance.StartCoroutine("GetImageData", byteData);
    }
	
	private bool Waited(float seconds)
	{
		timerMax = seconds;
	 
		timer += Time.deltaTime;
	 
		if (timer >= timerMax)
		{
			return true; //max reached - waited x - seconds
		}
	 
		return false;
	}
}