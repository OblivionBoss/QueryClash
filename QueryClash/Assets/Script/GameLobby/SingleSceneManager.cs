using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SingleSceneManager : MonoBehaviour
{
    public static SingleSceneManager singleSceneManager;
    public int rdb = 0;
    public int difficulty = 0;

    [SerializeField] private TMP_Dropdown rdbDropdown;
    [SerializeField] private TMP_Dropdown difficultyDropdown;

    private void Awake()
    {
        if (singleSceneManager == null)
        {
            singleSceneManager = this;
        }
        else
        {
            Destroy(singleSceneManager.gameObject);
            singleSceneManager = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Play()
    {
        rdb = rdbDropdown.value;
        difficulty = difficultyDropdown.value;

        SceneManager.LoadSceneAsync("SinglePlayerScene");
        StartCoroutine(DelayStartRDB());
    }

    private IEnumerator DelayStartRDB()
    {
        yield return new WaitForSeconds(1f);

        RDBManager rdbm = GameObject.Find("RDBManager").GetComponent<RDBManager>();
        if (rdb < rdbm.rdbName.Length)
            rdbm.StartRDB(rdbm.rdbName[rdb]);
        else
            rdbm.StartRDB(rdbm.rdbName[0]);
    }
}