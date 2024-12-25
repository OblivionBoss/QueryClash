using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

struct Token
{
    public bool newline;
    public SQLToken sqltoken;
}

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
    private int TokenPadding;
    public float FontSize;

    private List<Token> tokenList = new List<Token>();
    private List<SQLLine> lineList = new List<SQLLine>();
    //private (bool isActive, bool isNewline)[] SQLlineflag;

    private int cursorPosition = -1; // cursor at line 0 head
    private SQLCursor cursor;

    // Start is called before the first frame update
    void Start()
    {
        //SetupSize();
        //GenerateLine(0, -1, -1);

        //cursor = Instantiate<SQLCursor>(CursorPrefab, TokenScreen);
        //cursor.rectTransform.anchoredPosition = new Vector2(LineHeadWidth, -(0 * TokenHeight));
    }

    void Awake()
    {
        SetupSize();
        GenerateLine(0, -1, -1);

        cursor = Instantiate<SQLCursor>(CursorPrefab, TokenScreen);
        cursor.rectTransform.anchoredPosition = new Vector2(LineHeadWidth, -(0 * TokenHeight));
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
        TokenPadding = Mathf.RoundToInt(TokenPrefab.tokenText.preferredWidth);
        TokenPrefab.horizontalForRightPad.padding.right = TokenPadding;
        Debug.Log("TokenPrefab.horizontalForRightPad.padding.right = " + TokenPrefab.horizontalForRightPad.padding.right);

        // set Token height
        TokenHeight = TokenPrefab.tokenText.preferredHeight;
        Debug.Log("TokenHeight = " + TokenHeight);

        // setup line
        // setup line head size
        LineHeadWidth = 2 * TokenPadding;
        Debug.Log("LineHeadWidth = " + LineHeadWidth);
        LinePrefab.lineHead.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, LineHeadWidth);

        // setup line size
        LinePrefab.line.offsetMin = new Vector2(2 * TokenPadding, 0);
        Debug.Log("LinePrefab.line.offsetMin = " + LinePrefab.line.offsetMin);

        // set SQLLine height
        LinePrefab.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TokenHeight);

        // setup cursor
        CursorPrefab.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TokenHeight);
        CursorPrefab.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, LineHeadWidth / 6);
    }

    private void GenerateLines()
    {
        //SQLlineflag = new (bool isActive, bool isNewline)[MaxNumberOfLine];
        TokenScreen.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TokenHeight * MaxNumberOfLine);
        for (int i = 0; i < MaxNumberOfLine; i++)
        {
            SQLLine line = Instantiate<SQLLine>(LinePrefab, TokenScreen);
            line.lineNumber = i;
            line.rectTransform.anchoredPosition = new Vector2(0, -(i * TokenHeight));
            line.keyboardManager = this;

            //SQLlineflag[i] = (false, false);
        }
        //SQLlineflag[0] = (true, false);
    }

    private void GenerateLine(int linenum, int lineHeadToken, int lastToken)
    {
        SQLLine line = Instantiate<SQLLine>(LinePrefab, TokenScreen);
        line.lineNumber = linenum;
        line.rectTransform.anchoredPosition = new Vector2(0, -(linenum * TokenHeight));
        line.keyboardManager = this;
        line.transform.SetSiblingIndex(linenum);

        line.lineHeadToken = lineHeadToken;
        line.lastTokenOfLine = lastToken;
        lineList.Add(line);
    }

    private void DestroyLines()
    {
        while (lineList.Count > 1)
        {
            SQLLine line = lineList[lineList.Count - 1];
            lineList.RemoveAt(lineList.Count -1);
            Destroy(line.gameObject);
        }
        lineList[0].lineHeadToken = -1;
        lineList[0].lastTokenOfLine = -1;
    }

    private void SetCursorPosition(Vector2 newPosision)
    {
        cursor.rectTransform.anchoredPosition = newPosision;
    }
    
    private void UpdateTokenScreen()
    {
        DestroyLines();
        int line = 0;
        float totalTokenWidthAtCurrentLine = 0f;
        for (int i = 0; i < tokenList.Count; i++)
        {
            if (tokenList[i].newline)
            {
                line++;
                GenerateLine(line, i, -1);
                totalTokenWidthAtCurrentLine = 0f;

                if(cursorPosition == i) SetCursorPosition(new Vector2(LineHeadWidth, -(line * TokenHeight)));
            }
            else
            {
                Vector2 tokenPosition;
                float tokenWidth = tokenList[i].sqltoken.tokenText.preferredWidth + TokenPadding;
                float totalTokenWidth = totalTokenWidthAtCurrentLine + tokenWidth;
                if (LineHeadWidth + totalTokenWidth > TokenScreenWidth) // the newline in a token screen because the line length not enough
                {
                    line++;
                    GenerateLine(line, i - 1, i);
                    tokenPosition = new Vector2(LineHeadWidth, -(line * TokenHeight));
                    totalTokenWidthAtCurrentLine = tokenList[i].sqltoken.rectTransform.rect.width;

                    if (cursorPosition == i - 1) SetCursorPosition(new Vector2(LineHeadWidth, -(line * TokenHeight)));
                }
                else
                {
                    tokenPosition = new Vector2(LineHeadWidth + totalTokenWidthAtCurrentLine, -(line * TokenHeight));
                    lineList[line].lastTokenOfLine = i;
                    totalTokenWidthAtCurrentLine = totalTokenWidth;
                }
                tokenList[i].sqltoken.rectTransform.anchoredPosition = tokenPosition;
                tokenList[i].sqltoken.tokenIndex = i;

                if (cursorPosition == i)
                {
                    RectTransform r = tokenList[i].sqltoken.rectTransform;
                    //Debug.Log($"{r.anchoredPosition.x} {tokenWidth} {r.anchoredPosition.y}");
                    Vector2 pos = new Vector2(r.anchoredPosition.x + tokenWidth, r.anchoredPosition.y);
                    SetCursorPosition(pos);
                }
            }
        }
    }

    private void insertToken(Token t)
    {
        if (tokenList.Count == 0) // empty token screen
        {
            tokenList.Add(t);
            cursorPosition = 0;
        }
        else
        {
            if (cursorPosition == tokenList.Count - 1) tokenList.Add(t);
            else tokenList.Insert(cursorPosition + 1, t);
            cursorPosition++;
        }
    }

    public void AddToken(string str)
    {
        SQLToken token = Instantiate<SQLToken>(TokenPrefab, TokenScreen);
        token.AddKeyboardManager(this);
        token.tokenText.text = str;

        Token t = new Token();
        t.newline = false;
        t.sqltoken = token;
        insertToken(t);

        UpdateTokenScreen();
    }

    // the real newline (\n...)
    public void AddNewLine()
    {
        Token t = new Token();
        t.newline = true;
        t.sqltoken = null;
        insertToken(t);

        UpdateTokenScreen();
    }

    public void DeleteToken()
    {

    }

    public void OnClickToken(SQLToken token)
    {
        Debug.Log($"Token ({token.tokenText.text}, {token.tokenIndex}) is clicked");
        cursorPosition = token.tokenIndex;
        Vector2 pos = new Vector2(token.rectTransform.anchoredPosition.x + token.rectTransform.rect.width, token.rectTransform.anchoredPosition.y);
        SetCursorPosition(pos);
    }

    public void OnClickLine(SQLLine line)
    {
        Debug.Log($"Line {line.lineNumber} is clicked");
        if (line.lastTokenOfLine == -1) // line is empty (\n)
        {
            if (line.lineHeadToken == -1) // first line
            {
                cursorPosition = -1; // point to first line head
                SetCursorPosition(new Vector2(LineHeadWidth, 0)); // point to line head
            }
            else // new line (\n)
            {
                cursorPosition = line.lineHeadToken;
                SetCursorPosition(new Vector2(LineHeadWidth, -(line.lineNumber * TokenHeight))); // point to line head
            } 
        }
        else
        {
            //Debug.Log($"Token index {line.lastTokenOfLine} / {tokenList.Count} is last");
            cursorPosition = line.lastTokenOfLine;
            RectTransform r = tokenList[line.lastTokenOfLine].sqltoken.rectTransform;
            Vector2 pos = new Vector2(r.anchoredPosition.x + r.rect.width, r.anchoredPosition.y);
            SetCursorPosition(pos);
        }
    }

    public void OnClickLineHead(SQLLine line)
    {
        Debug.Log($"Line Head {line.lineNumber} is clicked");
        cursorPosition = line.lineHeadToken;
        SetCursorPosition(new Vector2(LineHeadWidth, -(line.lineNumber * TokenHeight)));
    }
}
