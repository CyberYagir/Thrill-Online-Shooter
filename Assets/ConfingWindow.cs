using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfingWindow : MonoBehaviour
{
    public Slider sens, quality, volume, fov;

    public TMP_Text sensT, qualityT, fovT;
    public Toggle drawParticles, drawTextures;

    public TMP_InputField sx, sy;


    ParticleSystem[] particles;

    int texold;
    public void changeVolume()
    {
        PlayerPrefs.SetFloat("Vol", volume.value);
        AudioListener.volume = PlayerPrefs.GetFloat("Vol");
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().Play();
    }

    private void Start()
    {
        GetComponent<AudioSource>().Stop();
        particles = FindObjectsOfType<ParticleSystem>();
        quality.maxValue = 5;
        sx.text = "" + Screen.width;
        sy.text = "" + Screen.height;
        if (PlayerPrefs.HasKey("Vol"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Vol");
            volume.value = PlayerPrefs.GetFloat("Vol");
            GetComponent<AudioSource>().Stop();
        }
        else
        {
            AudioListener.volume = 0.5f;
        }

        if (PlayerPrefs.HasKey("Qual"))
        {
            quality.value = PlayerPrefs.GetInt("Qual");
        }
        if (PlayerPrefs.HasKey("Fov"))
        {
            fov.value = PlayerPrefs.GetInt("Fov");
        }
        if (PlayerPrefs.HasKey("Sens"))
        {
            sens.value = PlayerPrefs.GetFloat("Sens");
        }
        if (PlayerPrefs.HasKey("Partic"))
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].gameObject.SetActive(PlayerPrefs.GetInt("Partic") == 1 ? true : false);
            }
            drawParticles.isOn = PlayerPrefs.GetInt("Partic") == 1 ? true : false;
        }
        if (PlayerPrefs.HasKey("Tex"))
        {
            if (PlayerPrefs.GetInt("Tex") == 1)
            {
                var meshes = FindObjectsOfType<MeshRenderer>();
                for (int i = 0; i < meshes.Length; i++)
                {
                    for (int j = 0; j < meshes[i].materials.Length; j++)
                    {
                        if (meshes[i].materials[j].mainTexture != null)
                        {
                            Texture2D tex = (Texture2D)(meshes[i].materials[j].GetTexture("_MainTex"));
                            print("tex = null");
                            Texture2D newTex = new Texture2D(1, 1);
                            newTex.SetPixel(0, 0, tex.GetPixel(0, 0));
                            newTex.Apply();
                            meshes[i].materials[j].mainTexture = newTex;
                            //Texture2D newTex = new Texture2D(1, 1);
                            //newTex.SetPixel(0, 0, tex.GetPixel(1,1));
                        }
                    }
                }
            }
            texold = PlayerPrefs.GetInt("Tex");
            drawTextures.isOn = PlayerPrefs.GetInt("Tex") == 1 ? true : false;
        }

    }
    public static Texture2D ToTexture2D(Texture texture)
    {
        return Texture2D.CreateExternalTexture(
            texture.width,
            texture.height,
            TextureFormat.RGBA32,
            false, false,
            texture.GetNativeTexturePtr());
    }
    private void Update()
    {
        QualitySettings.SetQualityLevel((int)quality.value);
        sensT.text = sens.value.ToString("0.00");
        qualityT.text = "" + QualitySettings.GetQualityLevel();
        fovT.text = "" + (int)fov.value;
    }

    public void Save()
    {
        QualitySettings.SetQualityLevel((int)quality.value);
        PlayerPrefs.SetInt("Qual", (int)quality.value);
        PlayerPrefs.SetFloat("Sens", sens.value);
        PlayerPrefs.SetInt("Partic", drawParticles.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Fov", (int)fov.value);

        PlayerPrefs.SetFloat("Vol", volume.value);
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].gameObject.SetActive(drawParticles.isOn);
        }
        PlayerPrefs.SetInt("Tex", drawTextures.isOn ? 1 : 0);
        if (sx.text.Replace(" ", "") == "")
        {
            sx.text = "640";
        }
        else
        {
            if (int.Parse(sx.text)<640)
            {
                sx.text = "640";
            }
            if (int.Parse(sx.text) > 1920)
            {
                sx.text = "1920";
            }
        }

        if (sy.text.Replace(" ", "") == "")
        {
            sy.text = "360";
        }
        else
        {
            if (int.Parse(sy.text) < 360)
            {
                sy.text = "360";
            }
            if (int.Parse(sy.text) > 1080)
            {
                sx.text = "1080";
            }
        }
        Screen.SetResolution(int.Parse(sx.text), int.Parse(sy.text), Screen.fullScreenMode);
        if (PlayerPrefs.GetInt("Tex") != texold)
        {
            Application.LoadLevel(0);
        }
    }
}
