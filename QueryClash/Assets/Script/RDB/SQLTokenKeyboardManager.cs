using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SQLTokenKeyboardManager : MonoBehaviour
{
    public Transform TokenScreen;
    public SQLToken TokenPrefab;
    public SQLLine LinePrefab;
    public SQLCursor CursorPrefab;

    public int MaxNumberOfLine = 20;
    public float TokenScreenWidth = 500;
    private float TokenScreenHeight;
    private float TokenHeight; // Assume every token has the same height and equal to line height 
    private float LineHeadWidth;
    public float FontSize;

    private List<(List<SQLToken> tokens, bool newlineFlag)> tokenLineList = new List<(List<SQLToken>, bool)>();

    private int cursorPosition;
    [SerializeField] private SQLCursor cursor;

    // Start is called before the first frame update
    void Start()
    {
        SetupSize();
        GenerateLines();

        cursor = Instantiate<SQLCursor>(CursorPrefab, TokenScreen);
        cursor.rectTransform.anchoredPosition = new Vector2(LineHeadWidth, -(0 * TokenHeight));

        SQLToken token = Instantiate<SQLToken>(TokenPrefab, TokenScreen);
        token.AddKeyboardManager(this);
        token.rectTransform.anchoredPosition = new Vector2(LineHeadWidth, -(0 * TokenHeight));
    }

    void Awake()
    {
        //SetupSize();
        //GenerateLines();

        //cursor = Instantiate<SQLCursor>(CursorPrefab, TokenScreen);
        //cursor.rectTransform.anchoredPosition = new Vector2(LineHeadWidth, -(0 * TokenHeight));

        //SQLToken token = Instantiate<SQLToken>(TokenPrefab, TokenScreen);
        //token.AddKeyboardManager(this);
        //token.rectTransform.anchoredPosition = new Vector2(LineHeadWidth, -(0 * TokenHeight));
    }

    private void SetupSize()
    {
        // setup token
        // set font
        TokenPrefab.tokenText.fontSize = FontSize;
        TokenPrefab.tokenText.text = ",";
        Debug.Log("TokenPrefab.tokenText.fontSize = " + TokenPrefab.tokenText.fontSize);

        // set pad right of token
        TokenPrefab.horizontalForRightPad.padding.right = Mathf.RoundToInt(TokenPrefab.tokenText.preferredWidth);
        Debug.Log("TokenPrefab.horizontalForRightPad.padding.right = " + TokenPrefab.horizontalForRightPad.padding.right);

        // set Token height
        TokenHeight = TokenPrefab.tokenText.preferredHeight;
        Debug.Log("TokenHeight = " + TokenHeight);

        // setup line
        // setup line head size
        LineHeadWidth = 2 * TokenPrefab.horizontalForRightPad.padding.right;
        Debug.Log("LineHeadWidth = " + LineHeadWidth);
        LinePrefab.lineHead.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, LineHeadWidth);

        // setup line size
        LinePrefab.line.offsetMin = new Vector2(2 * TokenPrefab.horizontalForRightPad.padding.right, 0);
        Debug.Log("LinePrefab.line.offsetMin = " + LinePrefab.line.offsetMin);

        // set SQLLine height
        LinePrefab.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TokenHeight);

        // setup cursor
        CursorPrefab.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TokenHeight);
        CursorPrefab.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, LineHeadWidth / 6);
    }

    private void GenerateLines()
    {
        TokenScreen.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TokenHeight * MaxNumberOfLine);
        for (int i = 0; i < MaxNumberOfLine; i++)
        {
            SQLLine line = Instantiate<SQLLine>(LinePrefab, TokenScreen);
            line.lineNumber = i;
            line.rectTransform.anchoredPosition = new Vector2(0, -(i * TokenHeight));
            line.SQLLineHead.AddKeyboardManager(this);
        }
    }

    private void SetCursorPosition(Vector2 newPosision)
    {
        cursor.rectTransform.anchoredPosition = newPosision;
    }

    public void AddToken(string str)
    {
        SQLToken token = Instantiate<SQLToken>(TokenPrefab, TokenScreen);
        token.AddKeyboardManager(this);
        token.tokenText.text = str;

        var width = token.tokenText.preferredWidth;

        token.rectTransform.anchoredPosition = new Vector2(LineHeadWidth, -(0 * TokenHeight));
    }

    // the real newline (\n...)
    private void AddNewLine()
    {

    }

    // the newline in a token screen because the line length not enough
    private void PlusNewLine()
    {

    }

    private void DeleteToken()
    {

    }

    public void OnClickToken(SQLToken token)
    {
        Debug.Log("Token is clicked");
        SetCursorPosition(new Vector2(LineHeadWidth, -(1 * TokenHeight)));
    }

    public void OnClickLine(SQLLine token)
    {
        Debug.Log($"Line {token.lineNumber} is clicked");
    }

    public void OnClickLineHead(SQLLine token)
    {
        Debug.Log($"Line Head {token.lineNumber} is clicked");
        if (token == null) Debug.Log("aaaaaaaaaaaaaaaa");
        if (cursor == null) Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
        SetCursorPosition(new Vector2(LineHeadWidth, -(token.lineNumber * TokenHeight)));
    }
}
