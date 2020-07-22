using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PdfCreater : MonoBehaviour
{
    public GameObject PdfReaderPrefab;
    public GameObject[] PdfImgs;
    GameObject pdfNow;


    public void CreatePdf(GameObject selectedImg)
    {
        pdfNow = Instantiate(PdfReaderPrefab);
        // print("streamingAssetsPath : " +Application.streamingAssetsPath);
        // print("dataPath : " + Application.dataPath);
        // print("persistentDataPath: " + Application.persistentDataPath);

        if (Application.platform == RuntimePlatform.Android)
        {
            string filePath = Application.persistentDataPath;
            switch (selectedImg.name)
            {
                case "Pdf1":
                    pdfNow.GetComponent<Paroxe.PdfRenderer.PDFViewer>().FilePath = "/storage/emulated/0/SendAnywhere/pdf1.pdf";
                    break;
                case "Pdf2":
                    pdfNow.GetComponent<Paroxe.PdfRenderer.PDFViewer>().FilePath = "/storage/emulated/0/SendAnywhere/pdf2.pdf";
                    break;
                case "Pdf3":
                    pdfNow.GetComponent<Paroxe.PdfRenderer.PDFViewer>().FilePath = "/storage/emulated/0/SendAnywhere/pdf3.pdf";
                    break;
            }
        }
        else
        {
            switch (selectedImg.name)
            {
                case "Pdf1":
                    pdfNow.GetComponent<Paroxe.PdfRenderer.PDFViewer>().FilePath = Application.dataPath + "/StreamingAssets/Loisboard _MAROMAV_cn_11.7.pdf";
                    break;
                case "Pdf2":
                    pdfNow.GetComponent<Paroxe.PdfRenderer.PDFViewer>().FilePath = Application.dataPath + "/StreamingAssets/arvrMarket.pdf";
                    break;
                case "Pdf3":
                    pdfNow.GetComponent<Paroxe.PdfRenderer.PDFViewer>().FilePath = Application.dataPath + "/StreamingAssets/funding.pdf";
                    break;
            }
        }

        pdfNow.transform.parent = transform;
        RectTransform rTran = pdfNow.GetComponent<RectTransform>();
        pdfNow.SetActive(true);
        rTran.localScale = new Vector3(1, 1, 1);
        rTran.offsetMin = new Vector2(0, 0);
        rTran.offsetMax = new Vector2(0, 0);
        rTran.position = transform.position;
        for (int i = 0; i < PdfImgs.Length; i++)
        {
            PdfImgs[i].SetActive(false);
        }

    }
}
