using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

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

    //public int MaxNumberOfLine = 20;
    public float TokenScreenWidth;
    public float TokenScreenHeight;
    public float FontSize;
    private float TokenHeight; // Assume every token has the same height and equal to line height 
    private float LineHeadWidth;
    private int TokenPadding;

    public RDBManager RDBManager;
    private GameObject queryErrorBox;
    public TextMeshProUGUI SQLtext;
    public QueryLogger queryLogger;
    public Timer timer;
    public SingleTimer singleTimer;

    public Color keywordColor;
    public Color functionColor;
    public Color operatorColor;
    public Color tablenameColor;
    public Color identifierColor;
    public Color stringColor;
    public Color numberColor;

    private List<Token> tokenList = new List<Token>();
    private List<SQLLine> lineList = new List<SQLLine>();

    private int cursorPosition = -1; // cursor at line 0 head
    private SQLCursor cursor;
    private RectTransform tokenScreenRect;

    //void Awake()
    //{
    //    Setup();
    //    GenerateLine(0, -1, -1);

    //    cursor = Instantiate<SQLCursor>(CursorPrefab, TokenScreen);
    //    cursor.rectTransform.anchoredPosition = new Vector2(LineHeadWidth, -(0 * TokenHeight));
    //}

    void Start()
    {
        Setup();
        GenerateLine(0, -1, -1);

        cursor = Instantiate<SQLCursor>(CursorPrefab, TokenScreen);
        cursor.rectTransform.anchoredPosition = new Vector2(LineHeadWidth, -(0 * TokenHeight));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma)) AddOperatorToken(",");
        if (Input.GetKeyDown(KeyCode.Return)) Execute();
        if (Input.GetKeyDown(KeyCode.Space)) AddNewLine();
        if (Input.GetKeyDown(KeyCode.Backspace)) DeleteToken();
        if (Input.GetKeyDown(KeyCode.Backslash)) DeleteAllToken();
        if (Input.GetKeyDown(KeyCode.Delete)) DeleteAllToken();
    }

    private void Setup()
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

        // setup tokenscreen
        tokenScreenRect = TokenScreen.GetComponent<RectTransform>();
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
        if (cursorPosition == -1) SetCursorPosition(new Vector2(LineHeadWidth, 0));
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
                    totalTokenWidthAtCurrentLine = tokenWidth;

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

        float requireHeight = lineList.Count * TokenHeight;
        tokenScreenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, requireHeight > TokenScreenHeight ? requireHeight : TokenScreenHeight);
    }

    public void Execute()
    {
        if (queryErrorBox != null) Destroy(queryErrorBox);
        StringBuilder stringBuilder = new StringBuilder();
        bool isFound = false, isAliasingError = false;
        string prevStringToken = "", currStringToken = "";
        foreach (var token in tokenList)
        {
            if (token.newline)
            {
                stringBuilder.Append('\n');
            }
            else
            {
                if (!isAliasingError && token.sqltoken.isString)
                {
                    if (isFound)
                    {
                        currStringToken = token.sqltoken.tokenText.text;
                        isAliasingError = true;
                    }
                    else
                    {
                        isFound = true;
                        prevStringToken = token.sqltoken.tokenText.text;
                    }
                } 
                else if (!isAliasingError) isFound = false;

                stringBuilder.Append(' ');
                stringBuilder.Append(token.sqltoken.tokenText.text);
                stringBuilder.Append(' ');
            }
        }
        string query_command = stringBuilder.ToString();
        //SQLtext.text = query_command;

        if (isAliasingError) AliasingError(prevStringToken, currStringToken, query_command);
        else RDBManager.Query(query_command);
    }

    private void AliasingError(string prevString, string currString, string query_command)
    {
        StringBuilder stringBuilder = new StringBuilder();
        queryErrorBox = RDBManager.GenerateQueryErrorBox($"SQL syntax error\naliasing not allow\nat \" {prevString} {currString} \"");
        RDBManager.queryStat.queryError++;


        if (RDBManager.isNetwork)
        {
            if (timer.isCountingDown.Value) stringBuilder.Append("#P-");
            else stringBuilder.Append("#B-");
            stringBuilder.Append(timer.timerText.text);
        }
        else
        {
            if (singleTimer.isCountingDown) stringBuilder.Append("#P-");
            else stringBuilder.Append("#B-");
            stringBuilder.Append(singleTimer.timerText.text);
        }
        // log G with query_command
        stringBuilder.Append("-G# {");
        stringBuilder.Append(query_command.Replace("\n", " ").Replace("\r", " "));
        stringBuilder.Append("}");
        if (queryLogger != null) queryLogger.LogQuery(stringBuilder.ToString());
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

    public void AddKeywordToken(string str)
    {
        AddToken(str, keywordColor, false);
    }

    public void AddOperatorToken(string str)
    {
        AddToken(str, operatorColor, false);
    }

    public void AddTableNameToken(string str)
    {
        AddToken(str, tablenameColor, true);
    }

    public void AddColumnNameToken(string str)
    {
        AddToken(str, identifierColor, true);
    }

    public void AddStringToken(string str)
    {
        AddToken("\'" + str + "\'", stringColor, true);
    }

    public void AddNumberToken(string str)
    {
        AddToken(str, numberColor, false);
    }

    public void AddToken(string str, Color color, bool isString)
    {
        SQLToken SQLtoken = Instantiate<SQLToken>(TokenPrefab, TokenScreen);
        SQLtoken.AddKeyboardManager(this);
        SQLtoken.tokenText.text = str;
        SQLtoken.tokenText.color = color;
        SQLtoken.isString = isString;

        Token token = new Token();
        token.newline = false;
        token.sqltoken = SQLtoken;
        insertToken(token);

        UpdateTokenScreen();
    }

    // the real newline (\n...)
    public void AddNewLine()
    {
        Token token = new Token();
        token.newline = true;
        token.sqltoken = null;
        insertToken(token);

        UpdateTokenScreen();
    }

    public void DeleteToken()
    {
        if (cursorPosition != -1)
        {
            Token token = tokenList[cursorPosition];
            if (!token.newline)
            {
                Destroy(token.sqltoken.gameObject);
            }
            tokenList.RemoveAt(cursorPosition);
            cursorPosition--;

            UpdateTokenScreen();
        }
        Debug.Log($"token left {tokenList.Count}");
    }

    public void DeleteAllToken()
    {
        if (tokenList.Count == 0) return;

        while (tokenList.Count > 0)
        {
            Token token = tokenList[tokenList.Count - 1];
            if (!token.newline)
            {
                Destroy(token.sqltoken.gameObject);
            }
            tokenList.RemoveAt(tokenList.Count - 1);
        }

        cursorPosition = -1;
        UpdateTokenScreen();
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

    //private void GenerateLines()
    //{
    //    TokenScreen.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TokenHeight * MaxNumberOfLine);
    //    for (int i = 0; i < MaxNumberOfLine; i++)
    //    {
    //        SQLLine line = Instantiate<SQLLine>(LinePrefab, TokenScreen);
    //        line.lineNumber = i;
    //        line.rectTransform.anchoredPosition = new Vector2(0, -(i * TokenHeight));
    //        line.keyboardManager = this;
    //    }
    //}
}