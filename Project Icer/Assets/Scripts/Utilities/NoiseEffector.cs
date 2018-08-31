using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseEffector : MonoBehaviour
{
    #region VARIABLES

    public Vector3 AxesToAffect;
    public float tickerx = 0;
    public float tickery = 0;
    public float NoiseEffectorSpeed = 1f;
    public float NoiseLerpPercent = 0.2f;
    public float NoiseScalar = 1f;

    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;

    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;


    public float DisplacementScale = 1.0f;
    public float xScale = 1.0f;

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    void Awake()
    {
        rend = GetComponent<Renderer>();
        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        if (rend) rend.material.mainTexture = noiseTex;
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    void Update()
    {
        float displacement = DisplacementScale * Mathf.PerlinNoise(Time.time * xScale, 0.0F);

        if (rend) CalcNoise();

        Vector3 localdisplacement = Vector3.zero;
        localdisplacement.x = (Mathf.PerlinNoise(tickerx, 0) - 0.5f) * NoiseScalar;
        localdisplacement.y = (Mathf.PerlinNoise(0, tickery) - 0.5f) * NoiseScalar;
        localdisplacement = Vector3.Lerp(transform.localPosition, localdisplacement, NoiseLerpPercent);
        tickerx += Time.deltaTime * NoiseEffectorSpeed;
        tickery += Time.deltaTime * NoiseEffectorSpeed;
        if (tickerx > pixWidth)
            tickerx = 0;
        if (tickery > pixHeight)
            tickery = 0;

        transform.localPosition = localdisplacement;
    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFICS

    void CalcNoise()
    {
        // For each pixel in the texture...
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }


    #endregion METHODS_SPECIFICS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_EVENTS



    #endregion METHODS_EVENTS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_COLLISION



    #endregion METHODS_COLLISION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_COROUTINES



    #endregion METHODS_COROUTINES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region GIZMOS

    //OnGizmo - private void OnDrawGizmos() { }

    #endregion GIZMOS
}
