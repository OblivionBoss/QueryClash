using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InfoDataManager : MonoBehaviour
{
    // Start is called before the first frame update
    private List<GameObject> infoDataList = new List<GameObject>();
    private int currentPageIndex = 0;


    void Start()
    {
        foreach (Transform child in transform)
        {
            infoDataList.Add(child.gameObject);
            Debug.Log($"Add {child.gameObject.name} complete");
        }
        ShowInfoPage();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowInfoPage()
    {
        infoDataList[currentPageIndex].SetActive(true);
    }

    public void SetCurrentPage(int pageIndex)
    {
        infoDataList[currentPageIndex].SetActive(false);
        this.currentPageIndex = pageIndex;
        ShowInfoPage();
    }

}
