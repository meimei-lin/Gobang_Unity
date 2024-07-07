using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Chess : MonoBehaviour
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
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 PointPos;
        if (Input.GetMouseButtonDown(0)) //步驟7.1
        {
            PointPos = Input.mousePosition; //步驟7.2
            if (isPlaying && PlaceChess(PointPos)) //步驟7
            {
                flag = 1 - flag; //步驟6.2
            }
            CheckWinFor();
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
    private bool PlaceChess(Vector3 PointPos)
    {
        float minDis = float.MaxValue; //最小距離
        Vector2 closestPos = Vector2.zero; //最近的位置
        int closestX = -1, closestY = -1; //最近位置的x、y座標的索引值
        //遍歷棋盤
        for(int i = 0; i < 15; i++)
        {
            for(int j = 0; j < 15; j++)
            {
                float dist = Distance(PointPos, chessPos[i][j]);
                if(dist < minGridDis / 2 && chessState[i][j] == 0) 
                {
                    minDis = dist;
                    closestPos = chessPos[i][j];
                    closestX = i;
                    closestY = j;
                }
            }
        }
        if(closestX != -1 && closestY != -1)
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
            //假設boardList=[1,-1,0,0,-1]，那使用Select(...)會傳回字元序列['X','O',' ',' ','O']
            //ToArray()就是把這個字元序列轉換成字串 "XO  O"
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
                string str = "BoardC[i + j]:";
                Debug.Log($"i: {i}, j: {j}, {str} boardC[{i + j}]: {string.Join(", ", boardC[i + j])}");
            }
        }
        //最後輸出結果
        Debug.Log("最後組合完結果");
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
        }
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
}
