using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
//using System.Diagnostics;
using UnityEngine.SocialPlatforms.Impl;

public class AIChess : MonoBehaviour
{//步驟1
    //棋盤上四個錨點的位置
    public GameObject LeftTop;
    public GameObject RightTop;
    public GameObject LeftBottom;
    public GameObject RightBottom;
    //按鈕的UI面板
    public GameObject buttomPanel;
    //主攝影機
    public Camera cam;
    //黑白棋圖片的紋理
    public Texture2D black;
    public Texture2D white;
    //黑白方勝利的圖片
    public Texture2D blackWin;
    public Texture2D whiteWin;
    //重新開始按鈕
    public Button restartBtn;
    //AI步驟4
    //黑白棋按鈕
    public Button blackBtn;
    public Button whiteBtn;
    //AI步驟4.1
    //用來標記玩家是選擇甚麼顏色的棋子，初始為null表示玩家尚未選擇棋子顏色
    public int? userPlay = null;
//步驟3
    //四個錨點在螢幕上的位置
    Vector3 LTPos;
    Vector3 RTPos;
    Vector3 LBPos;
    Vector3 RBPos;
//步驟5
    //儲存棋盤上每個交點的位置
    List<List<Vector2>> chessPos;
//步驟4
    //定義棋盤網格的寬度與高度
    float gridWidth = 1;
    float gridHeight = 1;
    //網格寬度和高度中取較小的那個
    float minGridDis;
    //儲存棋盤上的落子狀態
    List<List<int>> chessState;

    int winner = 0; //初始化為0，代表沒有獲勝方
    bool isPlaying = true; //代表遊戲是否正在進行中
//步驟6.2
    int flag = 0; //判斷目前是哪一方下棋

    Config config;

    // Start is called before the first frame update
    void Start()
    {//步驟3
        //計算錨點在螢幕上的位置
        LTPos = cam.WorldToScreenPoint(LeftTop.transform.position);
        RTPos = cam.WorldToScreenPoint(RightTop.transform.position);
        LBPos = cam.WorldToScreenPoint(LeftBottom.transform.position);
        RBPos = cam.WorldToScreenPoint(RightBottom.transform.position);
        //計算棋盤網格的寬度與高度
        gridWidth = (RTPos.x - LTPos.x) / 14;
        gridHeight = (LTPos.y - LBPos.y) / 14;
        //取較小的網格間距，確保網格是正方形的
        minGridDis = gridWidth < gridHeight ? gridWidth : gridHeight;
        /* if (gridWidth < gridHeight){
         *         minGridDis =  gridWidth;
         *  }else{
         *      minGridDis = gridHeight
         *  }*/
        
        //初始化成15 * 15大小的二維列表
        chessPos = new List<List<Vector2>>();
        chessState = new List<List<int>>();
        for (int i = 0; i < 15; i++)
        {
            chessPos.Add(new List<Vector2>());
            chessState.Add(new List<int>());
            for (int j = 0; j < 15; j++)
            {
                chessPos[i].Add(Vector2.zero);
                chessState[i].Add(0);
            }
        }
      //步驟4
        //計算棋盤上可以落子的位置
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                chessPos[i][j] = new Vector2(LBPos.x + gridWidth * i, LBPos.y + gridHeight * j);
            }
        }
        //添加重新開始按鈕的監聽器
        restartBtn.onClick.AddListener(Restart);
        //AI步驟6
        //添加選擇黑白棋按鈕的監聽器
        //() => chooseColor(0)是一個lambda運算式，是一個匿名函數
        /*因為AddListener本身只能接受不帶參數的函數，但chooseColor()是有帶參數的，
         * 所以使用lambda運算式把帶參數的函數封裝成一個不帶參數的匿名函數，
         * 其中的 "=>" 符號是lambda運算子，有"移到"的意思，就是左邊的()是匿名函數的參數列表，
         * 在這邊沒有參數，當按鈕被點擊時，會移到右邊執行chooseColor()
         */
        blackBtn.onClick.AddListener(() => chooseColor(0));
        whiteBtn.onClick.AddListener(() => chooseColor(1));

        config = GetComponent<Config>();
        if(config == null)
        {
            config = gameObject.AddComponent<Config>();
        }

    }
    //重新開始按鈕的函數
    private void Restart()
    {
        for (int i = 0; i < 15; i++)
        {
            for(int j = 0; j < 15; j++)
            {
                chessState[i][j] = 0; //將棋盤落子狀態設置為空
            }
        }
        isPlaying = true; //遊戲繼續進行
        winner = 0; //獲勝方設為0，代表沒有獲勝者
        flag = 0; //代表一開始都是黑棋先下
        userPlay = null;
    }
    //AI步驟4
    //選擇甚麼顏色的棋子的函數
    public void chooseColor(int color)
    {
        if(userPlay == null) //玩家尚未選擇棋子顏色
        {
            if(color == 0) //玩家選擇黑棋
            {
                userPlay = 0; //標記玩家是選黑棋
                flag = userPlay.Value; //flag會等於0，代表黑棋先下
            }
            else //玩家選擇白棋
            {
                userPlay = 1; //標記玩家是選白棋
                flag = 0; //AI回合，AI會先下黑棋
                AIFirstMove(); //AI下第一步棋的函數，等一下會在後面定義
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            //如果沒有獲勝方且是玩家回合且滑鼠按下了左鍵
            if (winner == 0 && flag == userPlay && Input.GetMouseButtonDown(0))
            {
                Vector3 PointPos = Input.mousePosition; //獲取玩家當前滑鼠點擊的位置
                if (PlaceChess(PointPos, true)) //玩家放置棋子
                {
                    flag = 1 - flag; //變更回合方
                    CheckWinFor();
                    if (isPlaying) //如果遊戲仍在進行(沒有贏家)
                    {
                        AITurn(); //換AI下棋
                    }
                }
            }
        }
    }
 
    void OnGUI()
    {//步驟5
        for(int i = 0; i < 15; i++)
        {
            for(int j = 0; j < 15; j++)
            {
                if(chessState[i][j] == 1) //繪製黑棋
                {
                    GUI.DrawTexture(new Rect(chessPos[i][j].x - gridWidth / 2, 
                        Screen.height - chessPos[i][j].y - gridHeight / 2, gridWidth, gridHeight),
                        black);
                }
                if (chessState[i][j] == -1) //繪製白棋
                {
                    GUI.DrawTexture(new Rect(chessPos[i][j].x - gridWidth / 2,
                        Screen.height - chessPos[i][j].y - gridHeight / 2, gridWidth, gridHeight),
                        white);
                }
  
            }
        }
        //顯示獲勝方的圖片
        if(winner == 1) //顯示黑棋獲勝的圖片
        {
            GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.15f, Screen.width * 0.2f,
                Screen.height * 0.25f), blackWin);
        }
        if (winner == -1) //顯示白棋獲勝的圖片
        {
            GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.15f, Screen.width * 0.2f,
                Screen.height * 0.25f), whiteWin);
        }
    }
//步驟6.1
    //計算兩點的歐幾里得距離
    private float Distance(Vector3 mPos, Vector2 gridPos)
    {
        return Mathf.Sqrt(Mathf.Pow(mPos.x - gridPos.x, 2) + Mathf.Pow(mPos.y - gridPos.y, 2));
    }
 //步驟6
    //根據玩家點擊的位置找到最近的棋盤位置下棋
    private bool PlaceChess(Vector3 PointPos, bool isPlayerTurn) 
    {
        float minDis = float.MaxValue; //最小距離
        //Vector2 closestPos = Vector2.zero; //最近的位置
        int closestX = -1, closestY = -1; //最近位置的x、y座標的索引值
        //遍歷棋盤
        for(int i = 0; i < 15; i++)
        {
            for(int j = 0; j < 15; j++)
            {
                float dist = Distance(PointPos, chessPos[i][j]);
                if (isPlayerTurn) //玩家回合
                {
                    if (dist < minGridDis / 2 && chessState[i][j] == 0)
                    {
                        minDis = dist;
                       // closestPos = chessPos[i][j];
                        closestX = i;
                        closestY = j;
                    }
                }
                else //AI回合時，直接找到最近的空位置
                {
                    if(dist < minDis)
                    {
                        minDis = dist;
                       // closestPos = chessPos[i][j];
                        closestX = i;
                        closestY = j;
                    }
                }
                
            }
        }
        if(closestX != -1 && closestY != -1 && chessState[closestX][closestY] == 0)
        {
            chessState[closestX][closestY] = flag == 0 ? 1 : -1;
            return true; //放置棋子成功
        }
        return false; // 放置棋子失敗
    }
//勝利步驟1
    //檢查五子連一起的獲勝函數
    //檢查豎方向的五子是否有連一起
    private int CheckWin(List<List<int>> board)
    {
        foreach(var boardList in board)
        {
            //假設boardList=[1,-1,0,0,-1]，那使用Select(...)會傳回IEnumerable<char>型態的字元序列['X','O',' ',' ','O']
            //ToArray()轉換成字元陣列的型態
            //new string就是把這個字元陣列轉換成字串 "XO  O"
            //勝利步驟1.1
            string boardRow = new string(boardList.Select(i => i == 1 ? 'X': (i == -1 ? 'O': ' ')).ToArray());
            //如果變數boardRow有包含字串"XXXXX"
            if (boardRow.Contains("XXXXX"))
            {
                //回傳1
                return 1; //黑棋獲勝
            //否則 如果變數boardRow有包含字串"OOOOO"
            }else if (boardRow.Contains("OOOOO"))
            {
                //回傳0
                return 0; //白棋獲勝
            }
        }
        //回傳-1
        return -1; //沒有獲勝方
    }
    //勝利步驟2
    //檢查橫、豎、正斜、反斜四個方向是否有五子連一起的函數
    private List<int> checkWinAll(List<List<int>> board)
    {
        //勝利步驟2.2
        //創建一個存反斜、正斜方向的二維列表
        List<List<int>> boardC = new List<List<int>>();
        List<List<int>> boardD = new List<List<int>>();
        //因為棋盤是15 * 15大小的，算斜方向總共會有29個
        for (int i = 0; i < 29; i++)
        {
            //分別表示包含29個空列表的二維列表
            boardC.Add(new List<int>());
            boardD.Add(new List<int>());
        }
        //遍歷棋盤，從左下角的(0,0)開始，依序往上遍歷，當遍歷到左上角(0,14)後，會再從第二列的(1,0)開始往上遍歷
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                boardC[i + j].Add(board[i][j]); //反斜方向
                boardD[i - j + 14].Add(board[i][j]); //正斜方向
               // string str = "BoardC[i + j]:";
               // Debug.Log($"i: {i}, j: {j}, {str} boardC[{i + j}]: {string.Join(", ", boardC[i + j])}");
            }
        }
        //最後輸出結果
       /* Debug.Log("最後組合完結果");
        Debug.Log("------------------------------------------");
        for (int i = 0; i < boardC.Count; i++)
        {
            string str = $"boardC[{i}]: ";
            foreach (var item in boardC[i])
            {
                str += item + " ";
            }
            Debug.Log(str);
        }
        Debug.Log("------------------------------------------");
        for (int i = 0; i < board.Count; i++)
        {
            string str = $"board[{i}]: ";
            foreach (var item in board[i])
            {
                str += item + " ";
            }
            Debug.Log(str);
        }*/
        return new List<int>
        {
            CheckWin(board),
            CheckWin(transpose(board)),
            CheckWin(boardC),
            CheckWin(boardD)
        };
    }

    private void CheckWinFor()
    {
        List<int> result = checkWinAll(chessState); //獲取棋盤目前落子狀態的勝利狀況
        if (result.Contains(0))
        {
            Debug.Log("白棋獲勝");
            winner = -1;
            isPlaying = false; //代表遊戲停止
           
        }else if (result.Contains(1))
        {
            Debug.Log("黑棋獲勝");
            winner = 1;
            isPlaying = false;
        }
    }
    //將列表做轉置的函數
    private List<List<int>> transpose (List<List<int>> board)
    {
        int rowMatrix = board.Count; //取得整個二維列表裡一維列表的數量
        int colMatrix = board[0].Count; //取得一維列表的元素數量
        //創建一個轉置後的二維列表
        List<List<int>> transposed = new List<List<int>>();

        for (int i = 0; i < colMatrix; i++)
        {
            List<int> newRow = new List<int>();

            for (int j = 0; j < rowMatrix; j++)
            {
                newRow.Add(board[j][i]);
            }
            transposed.Add(newRow);
        }
        return transposed;
    }
    //AI步驟5
    //AI下第一步棋的函數
    private void AIFirstMove()
    {
        int x = 7, y = 7; //代表在棋盤的正中間位置
        chessState[x][y] = 1; //在棋盤(7, 7)位置下黑棋，AI先手的話都讓它下在棋盤正中心
        flag = 1 - flag; //變更回合，換玩家下棋
    }
    //計算棋盤上每一行落子狀態的總分
    //只有豎方向
    private int Value (List<List<int>> board, List<(string, List<(string, int)>)> tempList,
                        List<Dictionary<string,List<Tuple<string, int>>>> valueModel, char chr)
    {
        int score = 0; //初始化分數為0

        //AI步驟6.1
        foreach(var row in board) //遍歷棋盤
        {
            //把遍歷到的內容的數字轉成對應的棋子字元
            string listStr = new string(row.Select(c => c == 1 ? 'X' : (c == -1 ? 'O' : ' ')).ToArray());
            //如果該行內容裡的目標棋子數量少於2個，就跳過這一行，繼續遍歷下一行
            if(listStr.Count(c => c == chr) < 2)
            {
                continue;
            }
            //AI步驟6.2
            //i用來遍歷listStr的起始位置
            for(int i = 0; i < listStr.Length - 5; i++)
            {
                //用來暫時存目前遍歷的行中識別到的棋型
                List<(int, (string, Tuple<string, int>))> temp = new List<(int, (string, Tuple<string, int>))>();
                //AI步驟6.3
                for(int j = 5; j < 12; j++)
                {
                    //如果超出本行長度，則跳出這層迴圈
                    if (i + j > listStr.Length)
                        break;
                    //用來存listStr截取的子字串
                    string s = listStr.Substring(i, j);
                    //當s.Count(c => c == chr)大於5時，我們只會取到5
                    int sNum = Math.Min(s.Count(c => c == chr), 5);
                    if (sNum < 2)
                        continue;
                    //AI步驟6.4
                    //要來處理自己設計的棋型了
                    foreach(var valueGroup in valueModel) //表示逐個遍歷valueModel中的每個字典
                    {
                        foreach(var item in valueGroup) //表示遍歷valueGroup字典中的每個Key-Value對
                        {
                            /*item.Value是List<Tuple<string, int>>，
                             * shape是List<Tuple<string, int>>中的每個Tuple<string, int>*/
                            foreach (var shape in item.Value) 
                            {
                                if(s == shape.Item1) //如果找到匹配的棋型
                                {
                                    //把起始位置的索引、棋型代號、Tuple<string, int>加到temp中
                                    temp.Add((i, (item.Key, shape)));
                                    break; //跳出這層迴圈
                                }
                            }
                        }
                    }
                }

                //AI步驟6.5
                if(temp.Count > 0) //如果temp不為空，代表有找到匹配的棋型字串
                {
                    //獲取temp中的得分最高的分數，這裡都是指當前遍歷到的行
                    int MaxScore = temp.Max(item => item.Item2.Item2.Item2);
                    //找到滿足第一個最高得分的棋型
                    var MaxShape = temp.First(item => item.Item2.Item2.Item2 == MaxScore);
                    tempList.Add((MaxShape.Item2.Item1, new List<(string, int)> { (MaxShape.Item2.Item2.Item1, MaxShape.Item2.Item2.Item2)}));
                    score += MaxScore; //將每行的最高分加到score
                }
            }
        }
        return score;
    }

    //AI步驟7
    //計算棋盤上橫、豎、正斜、反斜方向落子狀態的總分
    private int ValueAll(List<List<int>> board, List<(string, List<(string, int)>)> tempList,
                List<Dictionary<string, List<Tuple<string, int>>>> valueModel, int color)
    {
        char chr = (color == 1) ? 'X' : 'O';
        //跟CheckWinAll的正斜、反斜處理方式一樣
        //創建一個存反斜、正斜方向的二維列表
        List<List<int>> boardC = new List<List<int>>();
        List<List<int>> boardD = new List<List<int>>();
        //因為棋盤是15 * 15大小的，算斜方向總共會有29個
        for (int i = 0; i < 29; i++)
        {
            //分別表示包含29個空列表的二維列表
            boardC.Add(new List<int>());
            boardD.Add(new List<int>());
        }
        //遍歷棋盤，從左下角的(0,0)開始，依序往上遍歷，當遍歷到左上角(0,14)後，會再從第二列的(1,0)開始往上遍歷
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                boardC[i + j].Add(board[i][j]); //反斜方向
                boardD[i - j + 14].Add(board[i][j]); //正斜方向
            }
        }
        //獲取四個方向的得分
        int a = Value(board, tempList, valueModel, chr);
        int b = Value(transpose(board), tempList, valueModel, chr);
        int c = Value(boardC, tempList, valueModel, chr);
        int d = Value(boardD, tempList, valueModel, chr);
        return a + b + c + d; 
    }
    private Dictionary<int, string> chessName = new Dictionary<int, string> {
        { 1, "黑棋" },
        { -1, "白棋" }
    };

    //AI步驟8
    //根據總得分找到最佳位置的函數
    private (int, int, int) ValueChess(List<List<int>> board, int color)
    {
        //AI步驟8.1
        int opponentColor = (color == 1) ? -1 : (color == -1) ? 1 : 0; //設置對手的棋子顏色
        //創建臨時列表，用來存AI和對手的臨時數據
        List<(string, List<(string, int)>)> temp_list_ai = new List<(string, List<(string, int)>)>();
        List<(string, List<(string, int)>)> temp_list_opponent = new List<(string, List<(string, int)>)>();
        //計算當前局面的分數
        //落子前，AI的得分
        int scoreAI = ValueAll(board, temp_list_ai, color == 1 ? config.valueModelX : config.valueModelO, color);
        //落子前，對手的得分
        int scoreOpponent = ValueAll(board, temp_list_opponent, color == 1 ? config.valueModelO : config.valueModelX, opponentColor);

        //用來記錄最佳落子位置
        (int, int) bestMoveAI = (0, 0); //AI最佳落子位置的座標
        (int, int) bestMoveOpponent = (0, 0); //對手最佳落子位置座標
        (int, int) bestMoveOverall = (0, 0); //綜合考慮所有因素後的最佳落子位置座標
        (int, int) pos = (0, 0);
        int score = 0;
        //AI在所有可能的落子位置中，能獲得的最高分數
        int bestScoreAI = 0;
        //對手在所有可能的落子位置中，能獲得的最高分數
        int bestScoreOpponent = 0;
        //表示在所有可能的落子位置中，計算出的最高綜合得分差
        int bestScoreDiff = 0;

        List<(string, List<(string, int)>)> bestMoveListAI = new List<(string, List<(string, int)>)>();
        List<(string, List<(string, int)>)> bestMoveListOpponent = new List<(string, List<(string, int)>)>();

        //AI步驟8.2
        //獲取棋盤上已經有棋子的範圍
        (int minX, int maxX, int minY, int maxY) = GetChessRange(board);
        //擴展搜尋範圍
        //在已有棋子的範圍基礎上向外擴展2格，但不超出棋盤邊界
        int startX = Math.Max(0, minX - 2);
        int endX = Math.Min(14, maxX + 2);
        int startY = Math.Max(0, minY - 2);
        int endY = Math.Min(14, maxY + 2);
        //遍歷擴展後的搜索範圍內的每個位置
        for(int x = startX; x <= endX; x++)
        {
            for(int y = startY; y <= endY; y++)
            {
                //用來暫時存在評估某個特定落子位置時產生的數據
                //記錄在評估過程中識別出的棋型代號與棋型字串
                List<(string, List<(string, int)>)> tp_list_ai = new List<(string, List<(string, int)>)>();
                List<(string, List<(string, int)>)> tp_list_opponent = new List<(string, List<(string, int)>)>();
                if (board[x][y] != 0) continue; //如果該位置有棋子，就跳過

                //AI步驟的8.3的a.1
                //模擬AI落子
                board[x][y] = color;
                //AI在這個位置落子後的分數
                int scoreA = ValueAll(board, tp_list_ai, color == 1 ? config.valueModelX : config.valueModelO, color);
                //AI步驟的8.3的c.1
                //AI在這個位置落子後對手的分數
                int scoreAO = ValueAll(board, tp_list_opponent, color == 1 ? config.valueModelO : config.valueModelX, opponentColor);
                if(scoreA > bestScoreAI)
                {
                    bestMoveAI = (x, y);
                    bestMoveListAI = tp_list_ai;
                    bestScoreAI = scoreA;
                }
                //AI步驟的8.3的b.1
                //模擬對手落子
                board[x][y] = opponentColor;
                //對手在這個位置落子後得到的分數
                int scoreO = ValueAll(board, tp_list_opponent, color == 1 ? config.valueModelO : config.valueModelX, opponentColor);
                if(scoreO > bestScoreOpponent)
                {
                    bestMoveOpponent = (x, y);
                    bestMoveListOpponent = tp_list_opponent;
                    bestScoreOpponent = scoreO;
                }
                board[x][y] = 0; //恢復棋盤
                //AI步驟的8.3的d
                //計算綜合得分
                //scoreDiff就是對AI有利的分數
                //1.1 * AI分數增長 + (對手原分數 - AI落子後對手分數) + (對手預期最高分 - AI落子後對手分數)
                //之所以(對手預期最高分 - AI落子後對手分數)，是為了增加防守的邏輯
                //之所以設置1.1的係數，是為了鼓勵AI進攻
                int scoreDiff = (int)(1.1 * (scoreA - scoreAI) + scoreOpponent - scoreAO + scoreO - scoreAO);  
                if(scoreDiff > bestScoreDiff)
                {
                    bestMoveOverall = (x, y);
                    bestScoreDiff = scoreDiff;
                }
            }
        }

        //AI步驟的8.3的e.1
        //根據不同情況選擇最佳落子
        if (bestScoreAI >= 1000) //如果AI能直接獲勝，就選擇這個AI的最佳位置落子
        {
            Debug.Log("策略1棋面：");
            Debug.Log($"{chessName[color]}棋面：{PrintList(temp_list_ai)}");      
            Debug.Log($"{chessName[opponentColor]}棋面： {PrintList(temp_list_opponent)}");
            board[bestMoveAI.Item1][bestMoveAI.Item2] = color; //在該位置下AI的棋子
            score = bestScoreAI;
            int scoreOpponentE = ValueAll(board, temp_list_ai, color == 1 ? config.valueModelO : config.valueModelX, opponentColor);
            pos = bestMoveAI;
            board[bestMoveAI.Item1][bestMoveAI.Item2] = 0; //恢復棋盤
            Debug.Log("執行策略1，直接獲勝");
            Debug.Log($"{chessName[color]}最佳落子: 座標{pos}, {chessName[color]}得分{score}, {chessName[opponentColor]}得分{scoreOpponentE}");
            Debug.Log($"{chessName[opponentColor]}原分數{scoreOpponent}, 預期最高分數{bestScoreOpponent}");
            Debug.Log($"若{chessName[opponentColor]}落子{bestMoveOpponent}, {chessName[opponentColor]}棋型:");
            PrintList(bestMoveListOpponent);
            Debug.Log($"{chessName[color]}原分數{scoreAI}, 預期最高分數{bestScoreAI}");
            Debug.Log($"若{chessName[color]}落子{bestMoveAI}, {chessName[color]}棋型:");
            PrintList(bestMoveListAI);
            Debug.Log(new string('-', 60));
            //return (bestMoveAI.Item1, bestMoveAI.Item2, bestScoreAI);
        }
        //AI步驟的8.3的f
        else if (bestScoreOpponent >= 1000) //如果對手能直接獲勝，AI會下這個位置阻擋對手的落子
        {
            Debug.Log("策略2棋面:");
            Debug.Log($"{chessName[color]}棋面: {PrintList(temp_list_ai)}");  
            Debug.Log($"{chessName[opponentColor]}棋面: {PrintList(temp_list_opponent)}");
            board[bestMoveOpponent.Item1][bestMoveOpponent.Item2] = color;  //在該位置下AI的棋子
            temp_list_ai.Clear();
            score = ValueAll(board, temp_list_ai, color == 1 ? config.valueModelX : config.valueModelO, color);
            int scoreOpponentE = ValueAll(board, temp_list_opponent, color == 1 ? config.valueModelO : config.valueModelX, opponentColor);
            pos = bestMoveOpponent;
            board[bestMoveAI.Item1][bestMoveAI.Item2] = 0; //恢復棋盤
            Debug.Log("執行策略2，防守:防止對方獲勝");
            Debug.Log($"{chessName[color]}最佳落子: 座標{pos}, {chessName[color]}得分{score}, {chessName[opponentColor]}得分{scoreOpponentE}");
            Debug.Log($"{chessName[opponentColor]}原分數{scoreOpponent}, 預期最高分數{bestScoreOpponent}");
            Debug.Log($"若{chessName[opponentColor]}落子{bestMoveOpponent}, {chessName[opponentColor]}棋型:");
            PrintList(bestMoveListOpponent);
            Debug.Log($"{chessName[color]}原分數{scoreAI}, 預期最高分數{bestScoreAI}");
            Debug.Log($"若{chessName[color]}落子{bestMoveAI}, {chessName[color]}棋型:");
            PrintList(bestMoveListAI);
            Debug.Log(new string('-', 60));
            //return (bestMoveOpponent.Item1, bestMoveOpponent.Item2, score);
        }
        //AI步驟的8.3的g
        else //以上兩種都沒有，就選擇綜合得分最高的落子
        {
            Debug.Log("策略3棋面: ");
            Debug.Log($"{chessName[color]}棋面: {PrintList(temp_list_ai)}");
            Debug.Log($"{chessName[opponentColor]}棋面: {PrintList(temp_list_opponent)}");
            board[bestMoveOverall.Item1][bestMoveOverall.Item2] = color;  //在該位置下AI的棋子
            temp_list_ai.Clear();
            temp_list_opponent.Clear();
            pos = bestMoveOverall;
            score = ValueAll(board, temp_list_ai, color == 1 ? config.valueModelX : config.valueModelO, color);
            board[bestMoveAI.Item1][bestMoveAI.Item2] = opponentColor; //在該位置下對手的棋子
            int scoreOpponentE = ValueAll(board, temp_list_opponent, color == 1 ? config.valueModelO : config.valueModelX, opponentColor);
            board[bestMoveAI.Item1][bestMoveAI.Item2] = 0; //恢復棋盤
            Debug.Log($"{chessName[color]}原得分 {scoreAI}");
            Debug.Log($"{chessName[color]}得分 {score}");
            Debug.Log($"{chessName[opponentColor]}原得分 {scoreOpponent}");
            Debug.Log($"{chessName[opponentColor]}得分 {scoreOpponentE}");
            Debug.Log("執行策略3，防守:防守+進攻");
            Debug.Log($"{chessName[color]}最佳落子：座標{pos}，{chessName[color]}得分{score}，{chessName[opponentColor]}得分{scoreOpponentE}");
            Debug.Log($"{chessName[opponentColor]}原分數{scoreOpponent}，預期最高分數{bestScoreOpponent}");
            Debug.Log($"若{chessName[opponentColor]}落子{bestMoveOpponent}，{chessName[opponentColor]}棋型:{PrintList(bestMoveListOpponent)}");
            Debug.Log($"{chessName[color]}原分數{scoreAI}，預期最高分數{bestScoreAI}");
            Debug.Log($"若{chessName[color]}落子{bestMoveAI}，{chessName[color]}棋型:{PrintList(bestMoveListAI)}");
            Debug.Log(new string('-', 60));
            // return (bestMoveOverall.Item1, bestMoveOverall.Item2, bestScoreDiff);
        }
        return (pos.Item1, pos.Item2, score);
    }

    //得到棋盤上已經有放置棋子的範圍函數
    private (int, int, int, int) GetChessRange(List<List<int>> board)
    {
        int minX = 14, maxX = 0, minY = 14, maxY = 0;
        //遍歷棋盤
        for(int x = 0; x < 15; x++)
        {
            for(int y = 0; y < 15; y++)
            {
                if(board[x][y] != 0) //棋盤的該位置上有放置棋子
                {
                    //一直去做更新
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }
        return (minX, maxX, minY, maxY);
    }
    //AI步驟9 
    //AI下棋的函數
    private void AITurn()
    {
        //獲取最佳落子位置
        (int x, int y, int score) = ValueChess(chessState, userPlay == 1 ? 1 : -1);
        Vector3 bestMove = chessPos[x][y]; 
        PlaceChess(bestMove, false); //調用放置棋子的函數
        flag = 1 - flag; //變更回合方
        CheckWinFor(); //檢查勝利條件
    }
    private string PrintList(List<(string, List<(string, int)>)> list)
    {
        string output = "";
        foreach (var item in list)
        {
            // 拼接item.Item1和item.Item2
            string sublist = string.Join(", ", item.Item2.Select(subItem => $"({subItem.Item1}, {subItem.Item2})"));
            output += $"  {item.Item1}: [{sublist}],";
        }
        return output.TrimEnd(','); // 去掉最後一個逗號
    }
}
