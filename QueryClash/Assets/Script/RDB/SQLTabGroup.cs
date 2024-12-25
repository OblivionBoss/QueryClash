using System.Collections.Generic;
using UnityEngine;

public class SQLTabGroup : MonoBehaviour
{
    public List<SQLTabButton> tabs;
    public Color tabIdle = new Color(0.153f, 0.153f, 0.153f);
    public Color tabHover = new Color(0.25f, 0.25f, 0.25f);
    public Color tabActive = new Color(0.08627451f, 0.08627451f, 0.08627451f);
    public SQLTabButton selectedTab;
    public List<GameObject> pages;

    public void Subscribe(SQLTabButton button)
    {
        if (tabs == null)
        {
            tabs = new List<SQLTabButton>();
        }

        tabs.Add(button);
    }

    public void OnTabEnter(SQLTabButton button)
    {
        ResetTabs();
        if (tabs == null || selectedTab != button)
        {
            button.background.color = tabHover;
        }
    }

    public void OnTabExit(SQLTabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(SQLTabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.color = tabActive;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);
        }
    }

    public void ResetTabs()
    {
        foreach (SQLTabButton tab in tabs)
        {
            if (selectedTab != null && selectedTab == tab) continue;
            tab.background.color = tabIdle;
        }
    }
}