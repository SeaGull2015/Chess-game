using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class ManageBoard : MonoBehaviour
{
    struct Move
    {
        public int startx;
        public int starty;
        public int dx;
        public int dy;
        public Move(int tdx, int tdy, int sx, int sy)
        {
            dx = tdx;
            dy = tdy;
            startx = sx;
            starty = sy;
        }
    }
    // Start is called before the first frame update
    public GameObject square;
    public GameObject boardPiece;
    public float startpositionX = -3.5f;
    public float startpositionY = -3.5f;
    public bool needBoard = true;
    public Color lightSquareUpColorSelfy = new Color(20, 20, 20);
    public Color lightSquareUpColorEmpty = new Color(20, 20, 20);
    public Color lightSquareUpColorEnemy = new Color(20, 20, 20);
    public bool isWhiteAI = true;
    public bool isBlackAI = true;
    public bool isPlayerWhite = true;

    private string[,] board = new string[8,8];
    private SquareBehaviour[,] squares = new SquareBehaviour[8,8];
    private PieceBehaviour[,] pieces = new PieceBehaviour[8,8];
    private List<Move>[,] moves = new List<Move>[8,8];
    private bool whiteTurn = true;
    private string defStart = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private bool[] castles = new bool[4]
    {
        false, false, false, false
    }; // left black, right black, left white, right white.
    private System.Random rnd = new System.Random();
    void Start()
    {
        if (needBoard)
        {
            createBoard();
            board = FEN(defStart);
            putPieces(board);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void lightUpSquares(Vector3 where)
    {
        int x = Convert.ToInt32(where.x - startpositionX);
        int y = Convert.ToInt32(where.y - startpositionY);
        squares[x, y].lightUp(lightSquareUpColorSelfy);
        foreach (var move in moves[x, y]) {
            if (board[x + move.dx, y + move.dy] == "empty") squares[x + move.dx, y + move.dy].lightUp(lightSquareUpColorEmpty);
            else squares[x + move.dx, y + move.dy].lightUp(lightSquareUpColorEnemy);
        }
    }

    public void lightDownSquares(Vector3 where)
    {
        int x = Convert.ToInt32(where.x - startpositionX);
        int y = Convert.ToInt32(where.y - startpositionY);
        squares[x, y].updateColor();
        foreach (var move in moves[x, y])
        {
            squares[x + move.dx, y + move.dy].updateColor();
        }
    }

    bool isWhite(string s)
    {
        return char.IsLower(s[0]);
    }

    void createBoard()
    {
        for (int tY = 0; tY < 8; tY++)
        {
            for (int tX = 0; tX < 8; tX++)
            {
                GameObject sq = Instantiate(square, transform.position + new Vector3(tX + startpositionX, tY + startpositionY, +1), transform.rotation);
                sq.name = "square " + (tY*8+tX).ToString();
                SquareBehaviour sqBehav = sq.GetComponent<SquareBehaviour>();
                sqBehav.isLight = (tY + tX) % 2 == 0;
                sqBehav.updateColor();

                squares[tX, tY] = sqBehav;
            }
        }
    }

    string[,] FEN(string FENinp)
    {
        int x = 0, y = 0;
        string tempPiece = "";
        string[,] res = new string[8,8];
        Dictionary<char, string> pieceTypes = new Dictionary<char, string>
        {
            {'p', "pawn"},
            {'n', "knight"},
            {'b', "bishop"},
            {'r', "rook"},
            {'q', "queen"},
            {'k', "king"},
        };

        int stage = 0;
        foreach (char c in FENinp)
        {
            if (c == ' ')
            {
                stage++;
                continue;
            }
            if (stage == 0)
            {
                if (char.IsNumber(c))
                {
                    int t = int.Parse(c.ToString());
                    while (t > 0)
                    {
                        res[x, y] = "empty";
                        x++;
                        t--;
                    }
                }
                else if (pieceTypes.ContainsKey(char.ToLower(c)))
                {
                    tempPiece = pieceTypes[char.ToLower(c)];
                    if (char.IsUpper(c))
                    {
                        tempPiece = tempPiece.ToUpper(); // black
                    }
                    res[x, y] = tempPiece;
                    x++;
                }
                else if (c == '/')
                {
                    x = 0;
                    y++;
                }
                if (y > 7)
                {
                    throw new Exception("wrong fen string");
                }
            }
            if (stage == 1)
            {
                if (c == 'w') whiteTurn = true;
                else if (c == 'b') whiteTurn = false;
            }
            if (stage == 2) 
            { 
                if (c == '-')
                {
                    castles = new bool[4] { false, false, false, false };
                }
                else if (c == 'q') { castles[0] = true; }
                else if (c == 'k') { castles[1] = true; }
                else if (c == 'Q') { castles[2] = true; }
                else if (c == 'K') { castles[3] = true; }
            }
            // there is other stuff in FEN, but I don't want to implement it.
        }

        return res;
    }

    void putPieces(string[,] tboard)
    {
        for (int tY = 0; tY < tboard.GetLength(0); tY++)
        {
            for(int tX = 0; tX < tboard.GetLength(1); tX++)
            {
                if (tboard[tX, tY] == "empty") continue;
                GameObject piece = Instantiate(boardPiece, transform.position + new Vector3(tX + startpositionX, tY + startpositionY), transform.rotation);
                PieceBehaviour pcBehaviour = piece.GetComponent<PieceBehaviour>();
                pieces[tX, tY] = pcBehaviour;
                if (isWhite(tboard[tX, tY]))
                {
                    pcBehaviour.isWhite = true;
                }
                if (!pcBehaviour.setType(tboard[tX, tY].ToLower()))
                {
                    throw new Exception("bad type when putting down pieces");
                }
            }
        }
        nextMove();
    }

    void nextMove()
    {
        foreach (var piece in pieces)
        {
            if (piece == null) continue;
            if (piece.isWhite == whiteTurn && !((whiteTurn && piece.isWhite && isWhiteAI) || (!whiteTurn && !piece.isWhite && isBlackAI))) { // if the piece colour matches the turn colour and this colour isn't AI controlled
                piece.canMove = true;
            }
            else
            {
                piece.canMove = false;
            }
        }
        for (int i = 0; i < 8; i++) // specifically, this probably makes the previous check useless, because pieces without moves shouldn't be able to move
        { // actually no, the previous check controls player input, depending on whether it's ai turn or nay
            for (int j = 0; j < 8; j++) {
                if (isWhite(board[i, j]) == whiteTurn)
                {
                    moves[i, j] = getMoves(i, j);
                }
                else
                {
                    moves[i, j] = new List<Move>();
                }
            }
        }

        if ((whiteTurn && isWhiteAI) || (!whiteTurn && isBlackAI))
        {
            Move epicMove = randomOpponent(whiteTurn, board, moves);
            MakeMove(epicMove);
        }

        whiteTurn = !whiteTurn;
    }

    public void extractMove(Vector3 from, Vector3 to, PieceBehaviour who)
    {
        int posxFrom = Convert.ToInt32(from.x - startpositionX);
        int posyFrom = Convert.ToInt32(from.y - startpositionY);
        int posxTo = Convert.ToInt32(to.x - startpositionX);
        int posyTo = Convert.ToInt32(to.y - startpositionY);

        pieces[posxFrom, posyFrom] = null;
        pieces[posxTo, posyTo] = who;

        board[posxTo, posyTo] = board[posxFrom, posyFrom];
        board[posxFrom, posyFrom] = "empty";
        //pieces[posxTo, posyTo].setColor(Color.red);
        //squares[posxTo, posyTo].lightUp(Color.red);

        nextMove();
    }

    private Move directSlidingMove(int distance, int dir, int sx, int sy) // this kinda sucks
    {
        if (dir == 0) return new Move(0, distance, sx, sy);
        if (dir == 1) return new Move(distance, 0, sx, sy);
        if (dir == 2) return new Move(0, -distance, sx, sy);
        if (dir == 3) return new Move(-distance, 0, sx, sy);
        if (dir == 4) return new Move(distance, distance, sx, sy);
        if (dir == 5) return new Move(distance, -distance, sx, sy);
        if (dir == 6) return new Move(-distance, -distance, sx, sy);
        if (dir == 7) return new Move(-distance, distance, sx, sy);
        else throw new Exception("directMove() wrong input dir");
    }

    private List<Move> getMoves(int x, int y)
    {
        string type = board[x, y];
        type = type.ToLower();
        string[] sliders = { "bishop", "queen", "rook" };
        if (sliders.Contains(type)) return getSlidingMoves(x, y);
        else if (type == "pawn") return getPawnMoves(x, y);
        else if (type == "king") return getKingMoves(x, y);
        else if (type == "knight") return getKnightMoves(x, y);
        else /*throw new Exception("getMoves() bad type");*/ return new List<Move>();
    }

    private List<Move> getSlidingMoves(int x, int y)
    {
        List<Move> res = new List<Move>();
        string type = board[x, y];
        bool thisWhite = isWhite(type);
        type = type.ToLower();

        int[] dirs = dirLen(x, y);

        if (type == "bishop" || type == "queen") // the following code somewhat sucks, but as of now I don't have ideas how to implement it better
        {
            for (int i = 4; i < 8; i++)
            {
                for (int j = 1; j <= dirs[i]; j++)
                {
                    Move tMove = directSlidingMove(j, i, x, y);
                    if (board[x + tMove.dx, y + tMove.dy] == "empty") res.Add(tMove);
                    else if (isWhite(board[x + tMove.dx, y + tMove.dy]) == thisWhite) break;
                    else if (isWhite(board[x + tMove.dx, y + tMove.dy]) != thisWhite)
                    {
                        res.Add(directSlidingMove(j, i, x, y));
                        break;
                    }
                }
            }
        }
        if (type == "rook" || type == "queen")
        {
            for(int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= dirs[i]; j++) {
                    Move tMove = directSlidingMove(j, i, x, y);
                    if (board[x + tMove.dx, y + tMove.dy] == "empty") res.Add(tMove);
                    else if (isWhite(board[x + tMove.dx, y + tMove.dy]) == thisWhite) break;
                    else if (isWhite(board[x + tMove.dx, y + tMove.dy]) != thisWhite)
                    {
                        res.Add(directSlidingMove(j, i, x, y));
                        break;
                    }
                }
            }
        }

        return res;
    }
    
    private List<Move> getPawnMoves(int x, int y)
    {
        List<Move> res = new List<Move>();
        bool thisWhite = isWhite(board[x, y]);
        int dir = thisWhite ? 1 : -1;
        int yLim = thisWhite ? 7 : 0;

        if (y != yLim && board[x, y + dir] == "empty")
        {
            res.Add(new Move(0, dir, x, y));
        }

        if (y != yLim && x < 7 && board[x + 1, y + dir] != "empty" && thisWhite != isWhite(board[x + 1, y + dir])) // I would rather have y < 7 or y > 0, but it would be ugly, so y != yLim
        {
            res.Add(new Move(1, dir, x, y));
        }
        if (y != yLim && x > 0 && board[x - 1, y + dir] != "empty" && thisWhite != isWhite(board[x - 1, y + dir]))
        {
            res.Add(new Move(-1, dir, x, y));
        }

        if (false) // TODO: insert en passant here
        {

        }
        if (false) // TODO: insert upgrade behaviour here
        {

        }


        return res;
    }

    private List<Move> getKingMoves(int x, int y)
    {
        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        List<Move> res = new List<Move>();
        bool thisWhite = isWhite(board[x, y]);

        for (int i = 0; i < 8; i++)
        {
            if (x + dx[i] >= 0 && x + dx[i] < 8 &&
                y + dy[i] >= 0 && y + dy[i] < 8 &&
                (board[x + dx[i], y + dy[i]] == "empty" ||
                isWhite(board[x + dx[i], y + dy[i]]) != thisWhite))
            {
                res.Add(new Move(dx[i], dy[i], x, y));
            }
        }

        return res;
    }

    private List<Move> getKnightMoves(int x, int y)
    {
        List<Move> res = new List<Move>();
        bool thisWhite = isWhite(board[x, y]);

        int[] dx = { -2, -1, 1, 2, 2, 1, -1, -2 };
        int[] dy = { -1, -2, -2, -1, 1, 2, 2, 1 };

        for (int i = 0; i < 8; i++)
        {
            if (x + dx[i] >= 0 && x + dx[i] < 8 &&
                y + dy[i] >= 0 && y + dy[i] < 8 &&
                (board[x + dx[i], y + dy[i]] == "empty" ||
                isWhite(board[x + dx[i], y + dy[i]]) != thisWhite))
            {
                res.Add(new Move(dx[i], dy[i], x, y));
            }
        }

        return res;
    }
    private int[] dirLen(int x, int y)
    {
        int[] res = new int[8];
        res[0] = 7-y; // north
        res[1] = 7-x; // east
        res[2] = y; // south
        res[3] = x; // west
        res[4] = 7 - (x > y ? x : y); // NE: 7 - max(x, y)
        res[5] = ((7 - x) > y ? y : 7 - x); // SE: min(x, 7 - y) - if something is wrong, SE and NW might be swapped
        res[6] = (x > y ? y : x); // SW - just minimum between x and y
        res[7] = (x > (7 - y) ? 7 - y : x);  // NW - min(7 - x, y)
        return res;
    }

    private void eatPiece(int x, int y)
    {
        board[x, y] = "empty";
        Destroy(pieces[x, y].gameObject);
    }

    private void MakeMove(Move mv, List<Move> additionalTargets = null)
    {
        if (additionalTargets != null) // this should be a basis for en passant? idk
        {
            foreach (Move target in additionalTargets)
            {
                eatPiece(target.startx + target.dx, target.starty + target.dy);
            }
        }
        // TODO: add logic for castling aka rokirovka (duh)
        board[mv.startx + mv.dx, mv.starty + mv.dy] = board[mv.startx, mv.starty];
        board[mv.startx, mv.starty] = "empty";
        pieces[mv.startx + mv.dx, mv.starty + mv.dy] = pieces[mv.startx, mv.starty];
        pieces[mv.startx, mv.starty] = null;

        pieces[mv.startx + mv.dx, mv.starty + mv.dy].move(mv.dx, mv.dy);
        nextMove();
    }

    private List<Move> tryRND(bool thisWhite, string[,] brd, List<Move>[,] mvs, int sidePieces) {
        sidePieces = rnd.Next(sidePieces);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (sidePieces == 0) return mvs[i, j];
                if (thisWhite == isWhite(brd[i, j])) sidePieces--;
            }
        }
        return new List<Move>();
    }
    private Move randomOpponent(bool thisWhite, string [,] brd, List<Move>[,] mvs)
    {
        int sidePieces = 0;
        foreach (string pc in brd)
        {
            if (thisWhite == isWhite(pc)) sidePieces++;
        }

        for (int i = 0; i < 10; i++) // 10 tries to get a random move
        {
            List<Move> tmp = tryRND(thisWhite, brd, mvs, sidePieces);
            if (tmp.Count > 0)
            {
                return tmp[rnd.Next(tmp.Count)];
            }
        }

        return mvs[1, 1][1];
    }
}
