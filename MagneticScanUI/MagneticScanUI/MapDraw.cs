using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagneticScanUI
{
    public enum PathType : int
    {
        nil=-1,
        Forward = 0,
        Left = 1,
        Right = 2,
        Back = 3,
    }
    class PathNode
    {
        

        public PathNode[] paths=new PathNode[4];
        public int[] pathlenght = new int[4];
        public int pathCount;
        public int pathID;
        public string pathName;
        public int x, y;
        public int w;
        public bool isNew
        {
            get { return pathCount == 0; }
            private set { }
        }
        public bool isDead
        {
            get { return pathCount <= 1; }
            private set { }
        }

        public PathType this[PathNode node]
        {
            get { for (int i = 0; i < paths.Length; i++) if (paths[i] == node) return (PathType)i; return PathType.nil; }
            private set
            {
            }
        }

        public PathNode this[PathType type]
        {
            get { return paths[(int)type]; }
            set {
                paths[(int)type] = value;

                pathCount = 0;
                for (int i = 0; i < this.paths.Length; i++)if(this.paths[i]!=null) pathCount++;
            }
        }

        public void update()
        {
            pathCount = 0;
            for (int i = 0; i < this.paths.Length; i++) if (this.paths[i] != null) pathCount++;
        }

        public PathNode(int id):this(id,"节点"+id)
        {

        }

        public PathNode(int id,string name)
        {
            pathID = id;
            pathName = name;
        }

        public void addPath(PathNode node,PathType dir)
        {
            this.paths[(int)dir] = node;
            pathCount = 0;
            for (int i = 0; i < this.paths.Length; i++) pathCount++;
        }



        public List<PathNode> gotoNode(PathNode node,List<PathNode> list=null)
        {
            List<PathNode> nodes=list;
            if (nodes == null)
            {
                nodes = new List<PathNode>();
            }
            for (int i = 0; i < paths.Length; i++)
            {
                if (this.paths[i] != null)
                {
                    if (this.paths[i] == node)
                    {
                        nodes.Add(this);
                        nodes.Add(this.paths[i]);
                        return nodes;
                    }
                    if (this.paths[i].isDead) continue;
                    if (nodes.Contains(this.paths[i])) continue;
                    nodes.Add(this);
                    if (this.paths[i].gotoNode(node, nodes) != null)
                        return nodes;
                    nodes.Remove(this);
                }
            }
            return null;
        }

        public List<PathNode> getEndNode(List<PathNode> list = null, List<PathNode> point = null)
        {
            List<PathNode> nodes = list;
            if (nodes == null)
            {
                nodes = new List<PathNode>();
                point = new List<PathNode>();
                //nodes.Add(this);
            }
            for (int i = 0; i < paths.Length; i++)
            {
                if (this.paths[i] != null)
                {
                    if (this.paths[i].isDead)
                    {
                        point.Add(this.paths[i]);
                        continue;
                    }
                    if (nodes.Contains(this.paths[i])) continue;
                    nodes.Add(this);
                    this.paths[i].getEndNode(nodes, point);
                    nodes.Remove(this);
                }
            }
            return point;
        }

        public List<PathNode> getAllNode(List<PathNode> list = null)
        {
            List<PathNode> nodes = list;
            if (nodes == null)
            {
                nodes = new List<PathNode>();
            }
            for (int i = 0; i < paths.Length; i++)
            {
                if (this.paths[i] != null)
                {
                    if (nodes.Contains(this.paths[i])) continue;
                    nodes.Add(this.paths[i]);
                    this.paths[i].getAllNode(nodes);
                }
            }
            return nodes;
        }

        public override string ToString()
        {
            return pathName;
        }

    }

    class PathDir
    {
        public PathType Forward = PathType.Forward;
        public PathType Left = PathType.Left;
        public PathType Right = PathType.Right;
        public PathType Back = PathType.Back;

        public PathDir()
        {

        }

        public PathDir(PathType f, PathType l, PathType r, PathType b)
        {
            Forward = f;
            Left = l;
            Right = r;
            Back = b;
        }

        public PathType this[PathType type]
        {
            get
            {
                switch (type)
                {
                    case PathType.Forward: return Forward;
                    case PathType.Left: return Left;
                    case PathType.Right: return Right;
                    case PathType.Back: return Back;
                }
                return PathType.nil;
            }
            private set { }
        }
        public PathType getDir(PathType type)
        {
            if (Forward == type) return PathType.Forward;
            if (Left == type) return PathType.Left;
            if (Right == type) return PathType.Right;
            if (Back == type) return PathType.Back;
            return PathType.nil;
        }

        public void Reset()
        {
            Forward = PathType.Forward;
            Left = PathType.Left;
            Right = PathType.Right;
            Back = PathType.Back;
        }
        public void Rotate(PathType type)
        {
            PathType t;
            switch (type)
            {
                case PathType.Forward: break;
                case PathType.Left:
                    t = Forward;
                    Forward = Left;
                    Left = Back;
                    Back = Right;
                    Right = t;
                    break;
                case PathType.Right:
                    t = Left;
                    Left = Forward;
                    Forward = Right;
                    Right = Back;
                    Back = t;
                    break;
                case PathType.Back:
                    t = Left;
                    Left = Forward;
                    Forward = Right;
                    Right = Back;
                    Back = t;
                    t = Left;
                    Left = Forward;
                    Forward = Right;
                    Right = Back;
                    Back = t;
                    break;
            }
        }

        public PathDir clone()
        {
            return new PathDir(this.Forward,this.Left,this.Right,this.Back);
        }

    }

    class MapDraw
    {
        Bitmap map;
        Graphics Draw;
        Pen norpen = new Pen(Color.Black,3);
        Pen deadpen = new Pen(Color.Gray, 3);
        Pen newpen = new Pen(Color.Blue, 3);
        Pen targetpen = new Pen(Color.Red, 3);
        Pen PathPen = new Pen(Color.GreenYellow, 1);

        List<PathNode> TargetPath = new List<PathNode>();
        List<PathType> TargetDirList = new List<PathType>();
        List<PathNode> NodeList=new List<PathNode>();
        List<PathNode> EndPathList;
        PathDir findDir=new PathDir();
        Font strFont = new Font("宋体", 9);    
        int xpos, ypos;
        int width=30;
        List<PathNode> allPath = new List<PathNode>();

        int nodescount = 0;
        PathNode nodes;

        PathNode TargetNode;
        PathNode lastNode;
        PathType lastPath;
        bool TurnBack = false;
        bool isOver = false;
        PathDir LastDir=new PathDir();

        public PathDir LastPathDir
        {
            get { return LastDir.clone(); }
            set
            {
                if (value != null)
                    LastDir = value.clone();
            }
        }

        Stack<PathNode> searchStack = new Stack<PathNode>();
        public MapDraw(int w,int h)
        {
            map = new Bitmap(w, h);
            Draw = Graphics.FromImage(map);
            Draw.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        void DrawTargetPath()
        {
            if (TargetDirList.Count == 0) return;
            PathNode node=null;
            foreach(PathNode n in TargetPath)
            {
                if(node==null)
                {
                    node = n;
                    continue;
                }
                Draw.DrawLine(PathPen, node.x, node.y, n.x, n.y);
                node = n;
            }
        }

        void DrawPath(int x,int y,PathNode node,PathType t=PathType.nil)
        {
            PathNode n;
            Pen pen = norpen;
            int ax=0, ay=0;
            int w = node.w != 0 ? node.w : width;
            node.x = x;
            node.y = y;
            if (t != PathType.nil)
            {
                if (node[t] != null)
                {
                    n = node[t];
                    switch (findDir[t])
                    {
                        case PathType.Forward:
                            ax = 0;
                            ay = -w;
                            break;
                        case PathType.Left:
                            ax = -w;
                            ay = 0;
                            break;
                        case PathType.Right:
                            ax = w;
                            ay = 0;
                            break;
                        case PathType.Back:
                            ax = 0;
                            ay = w;
                            break;
                    }
                    if (!n.isDead)
                    {
                        Draw.DrawLine(norpen, x, y, x + ax, y + ay);
                        findDir.Rotate(t);
                        NodeList.Add(node);
                        DrawPath(x + ax, y + ay, n);
                        NodeList.Remove(node);
                        findDir.Rotate(t);
                        findDir.Rotate(t);
                        findDir.Rotate(t);
                        Draw.DrawEllipse(norpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                        if (lastNode == n)
                            Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    }
                    else if (n.isNew)
                    {
                        Draw.DrawLine(newpen, x, y, x + ax, y + ay);
                        n.x = x+ax;
                        n.y = y+ay;
                    }
                    else
                    {
                        Draw.DrawLine(deadpen, x, y, x + ax, y + ay);
                        Draw.DrawEllipse(deadpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                        if (lastNode == n)
                            Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                        Draw.DrawString(n.pathName, strFont, Brushes.Black, x + ax + 5, y + ay + 5);
                        n.x = x + ax;
                        n.y = y + ay;
                    }
                }
                return;
            }
            if(node[findDir.Right]!=null&&!NodeList.Contains(node[findDir.Right]))
            {
                n = node[findDir.Right];
                switch (findDir.Right)
                {
                    case PathType.Forward:
                        ax = 0;
                        ay = -w;
                        break;
                    case PathType.Left:
                        ax = -w;
                        ay = 0;
                        break;
                    case PathType.Right:
                        ax = w;
                        ay = 0;
                        break;
                    case PathType.Back:
                        ax = 0;
                        ay = w;
                        break;
                }
                if (!n.isDead)
                {
                    Draw.DrawLine(norpen, x, y, x + ax, y + ay);
                    findDir.Rotate(PathType.Right);
                    NodeList.Add(node);
                    DrawPath(x + ax, y + ay, n);
                    NodeList.Remove(node);
                    findDir.Rotate(PathType.Left);
                    Draw.DrawEllipse(norpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    if (lastNode == n)
                        Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    
                        
                }
                else if (n.isNew)
                {
                    Draw.DrawLine(newpen, x, y, x + ax, y + ay);
                    n.x = x + ax;
                    n.y = y + ay;
                }
                else
                {
                    Draw.DrawLine(deadpen, x, y, x + ax, y + ay);
                    Draw.DrawEllipse(deadpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    if (lastNode == n)
                        Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    Draw.DrawString(n.pathName, strFont, Brushes.Black, x + ax + 5, y + ay + 5);
                    n.x = x + ax;
                    n.y = y + ay;
                }
            }
            if (node[findDir.Forward] != null && !NodeList.Contains(node[findDir.Forward]))
            {
                n = node[findDir.Forward];
                switch (findDir.Forward)
                {
                    case PathType.Forward:
                        ax = 0;
                        ay = -w;
                        break;
                    case PathType.Left:
                        ax = -w;
                        ay = 0;
                        break;
                    case PathType.Right:
                        ax = w;
                        ay = 0;
                        break;
                    case PathType.Back:
                        ax = 0;
                        ay = w;
                        break;
                }
                if (!n.isDead)
                {
                    Draw.DrawLine(norpen, x, y, x + ax, y + ay);
                    NodeList.Add(node);
                    DrawPath(x + ax, y + ay, n);
                    NodeList.Remove(node);
                    Draw.DrawEllipse(norpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    if (lastNode == n)
                        Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));


                }
                else if (n.isNew)
                {
                    Draw.DrawLine(newpen, x, y, x + ax, y + ay);
                    n.x = x + ax;
                    n.y = y + ay;
                }
                else
                {
                    Draw.DrawLine(deadpen, x, y, x + ax, y + ay);
                    Draw.DrawEllipse(deadpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    if (lastNode == n)
                        Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    Draw.DrawString(n.pathName, strFont, Brushes.Black, x + ax + 5, y + ay + 5);
                    n.x = x + ax;
                    n.y = y + ay;
                }
            }
            if (node[findDir.Left] != null && !NodeList.Contains(node[findDir.Left]))
            {
                n = node[findDir.Left];
                switch (findDir.Left)
                {
                    case PathType.Forward:
                        ax = 0;
                        ay = -w;
                        break;
                    case PathType.Left:
                        ax = -w;
                        ay = 0;
                        break;
                    case PathType.Right:
                        ax = w;
                        ay = 0;
                        break;
                    case PathType.Back:
                        ax = 0;
                        ay = w;
                        break;
                }
                if (!n.isDead)
                {
                    Draw.DrawLine(norpen, x, y, x + ax, y + ay);
                    findDir.Rotate(PathType.Left);
                    NodeList.Add(node);
                    DrawPath(x + ax, y + ay, n);
                    NodeList.Remove(node);
                    findDir.Rotate(PathType.Right);
                    Draw.DrawEllipse(norpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    if (lastNode == n)
                        Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));


                }
                else if (n.isNew)
                {
                    Draw.DrawLine(newpen, x, y, x + ax, y + ay);
                    n.x = x + ax;
                    n.y = y + ay;
                }
                else
                {
                    Draw.DrawLine(deadpen, x, y, x + ax, y + ay);
                    Draw.DrawEllipse(deadpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    if (lastNode == n)
                        Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    Draw.DrawString(n.pathName, strFont, Brushes.Black, x + ax + 5, y + ay + 5);
                    n.x = x + ax;
                    n.y = y + ay;
                }
            }
        }

        public List<PathNode> getEndPoint()
        {
            return EndPathList;
        }

        public Bitmap Update()
        {
            xpos = map.Width / 2;
            ypos = map.Height/2 ;
            Draw.Clear(Color.White);
            NodeList.Clear();
            DrawPath(xpos,ypos,nodes);
            findDir.Rotate(PathType.Back);
            DrawPath(nodes[PathType.Forward].x, nodes[PathType.Forward].y, nodes[PathType.Forward]);
            DrawTargetPath();
            findDir.Reset();
            return map;
        }

        public bool MoveToPoint(PathNode node)
        {
            if (node.isNew || !node.isDead) return false;
            PathDir dir = new PathDir();
            for(int i=0;i< node.paths.Length;i++)
                if(node.paths[i]!=null)
                {
                    dir.Rotate((PathType)i);
                    break;
                }
            dir.Rotate(PathType.Back);
            LastPathDir = dir;
            lastNode = node;
            return true;
        }

        public bool setTargetPoint(PathNode node)
        {
            TargetDirList.Clear();
            if (lastNode == null) return false;
            if (node.isNew || !node.isDead) return false;
            if (lastNode == node) return false;
            TargetPath = lastNode.gotoNode(node);
            if (TargetPath == null) return false;
            PathDir dir = LastPathDir;
            PathNode nt = null;
            foreach (PathNode n in TargetPath)
            {
                PathType type;
                if (nt == null)
                {
                    nt = n;
                    continue;
                }
                type = dir.getDir(nt[n]);
                dir.Rotate(type);
                TargetDirList.Add(type);
                nt = n;
            }
            return true;
        }

        public List<PathType> getTargetPath()
        {
            return TargetDirList;
        }


        public void SearchInit()
        {
            nodescount = 0;
            isOver = false;
            TargetNode = nodes = new PathNode(nodescount++);
            searchStack.Clear();
            searchStack.Push(nodes);
            nodes[PathType.Forward] = new PathNode(nodescount++);
            nodes[PathType.Forward][PathType.Back] = nodes;
            lastNode = nodes[PathType.Forward];
            LastDir.Reset();
            TurnBack = false;
        }
        public int SearchCheck(int lastPathSelect)
        {
            int PathOut = -1;
            if (!TurnBack)
            {
                lastPath = (PathType)PathOut;
                if ((lastPathSelect & 2) != 0)
                {
                    lastNode[LastDir.Left] = new PathNode(nodescount++);
                    PathOut = (int)PathType.Left;
                }
                if ((lastPathSelect & 1) != 0)
                {
                    lastNode[LastDir.Forward] = new PathNode(nodescount++);
                    PathOut = (int)PathType.Forward;
                }
                if ((lastPathSelect & 4) != 0)
                {
                    lastNode[LastDir.Right] = new PathNode(nodescount++);
                    PathOut = (int)PathType.Right;
                }
                if (PathOut < 0)
                {
                    lastNode = searchStack.Pop();
                    PathOut = (int)PathType.Back;
                    LastDir.Rotate(PathType.Back);
                    TurnBack = true;
                }
                else
                {
                    searchStack.Push(lastNode);
                    lastPath = LastDir[(PathType)PathOut];
                    LastDir.Rotate((PathType)PathOut);
                    lastNode[lastPath][LastDir.Back] = lastNode;
                    lastNode = lastNode[lastPath];
                }
            }
            else
            {
                PathOut = -1;
                if ((lastPathSelect & 2) != 0)
                {
                    if (lastNode[LastDir.Left] != null)
                        if (lastNode[LastDir.Left].isNew)
                            PathOut = (int)PathType.Left;
                }
                if ((lastPathSelect & 1) != 0)
                {
                    if (lastNode[LastDir.Forward] != null)
                        if (lastNode[LastDir.Forward].isNew)
                            PathOut = (int)PathType.Forward;
                }
                if ((lastPathSelect & 4) != 0)
                {
                    if (lastNode[LastDir.Right] != null)
                        if (lastNode[LastDir.Right].isNew)
                            PathOut = (int)PathType.Right;
                }
                if (PathOut < 0)
                {
                    if (searchStack.Count > 1)
                    {
                        TargetNode = lastNode;
                        lastPath = lastNode[searchStack.Pop()];
                        PathOut = (int)LastDir.getDir(lastPath);
                        lastNode = lastNode[lastPath];
                        LastDir.Rotate((PathType)PathOut);
                    }
                    else
                    {
                        if (!isOver)
                        {
                            isOver = true;
                        }
                        else
                        {
                            UpdateEndPoint();
                            return -1;
                        }
                        if (lastNode[PathType.Back].isDead)
                        {
                            searchStack.Push(lastNode);
                            PathOut = (int)LastDir.getDir(lastNode[lastNode[PathType.Back]]);
                            LastDir.Rotate((PathType)PathOut);
                            lastNode = lastNode[PathType.Back];
                            TurnBack = false;
                        }
                        
                    }
                }
                else
                {
                    TargetNode = lastNode;
                    searchStack.Push(lastNode);
                    lastPath = LastDir[(PathType)PathOut];
                    LastDir.Rotate((PathType)PathOut);
                    lastNode[lastPath][LastDir.Back] = lastNode;
                    lastNode = lastNode[lastPath];
                    TurnBack = false;
                }
            }
            return PathOut;
        }

        public void UpdateEndPoint()
        {
            EndPathList = nodes.getEndNode();
        }

        public void setNode(PathNode node)
        {
            nodes = node;
            UpdateEndPoint();
            lastNode=(lastNode=node[PathType.Forward])!=null?lastNode:node;
        }
        public Stream toBin(Stream s)
        {
            List<PathNode> allPath = nodes.getAllNode();
            int type = allPath.Count;
            int index;
            allPath.Sort((PathNode a, PathNode b) => { return a.pathID > b.pathID ? 1 : a.pathID == b.pathID ? 0 : -1; });
            s.WriteByte((byte)(type >> 8));
            s.WriteByte((byte)type);
            if (type < 256)
            {
                foreach (PathNode node in allPath)
                {
                    index = node.pathID;
                    s.WriteByte((byte)index);
                    for (int i = 0; i < node.paths.Length; i++)
                    {
                        index = allPath.IndexOf(node.paths[i]);
                        s.WriteByte((byte)index);
                        index = node.pathlenght[i];
                        s.WriteByte((byte)index);
                    }
                }
            }
            else
            {
                foreach (PathNode node in allPath)
                {
                    index = node.pathID;
                    s.WriteByte((byte)(index >> 8));
                    s.WriteByte((byte)index);
                    for (int i = 0; i < node.paths.Length; i++)
                    {
                        index = allPath.IndexOf(node.paths[i]);
                        s.WriteByte((byte)(index >> 8));
                        s.WriteByte((byte)index);
                        index = node.pathlenght[i];
                        s.WriteByte((byte)(index >> 8));
                        s.WriteByte((byte)index);
                    }
                }
            }
            return s;
        }

        public PathNode toNode(Stream s)
        {
            PathNode node;
            List<PathNode> allPath = new List<PathNode>();
            int index;
            int len = (s.ReadByte() * 256 + s.ReadByte());
            if (len >= 256)
            {
                for (int i = 0; i < len; i++)
                {
                    allPath.Add(new PathNode(i));
                }
                for (int i = 0; i < len; i++)
                {
                    node = allPath[i];
                    node.pathID = (256 * s.ReadByte() + s.ReadByte());
                    for (int j = 0; j < node.paths.Length; j++)
                    {
                        index = node.pathID = (256 * s.ReadByte() + s.ReadByte());
                        if (index!=(-1&0xffff))
                            node.paths[j] = allPath[index];
                        node.pathlenght[j] = (256 * s.ReadByte() + s.ReadByte());
                    }
                }
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    allPath.Add(new PathNode(i));
                }
                for (int i = 0; i < len; i++)
                {
                    node = allPath[i];
                    node.pathID = (s.ReadByte());
                    for (int j = 0; j < node.paths.Length; j++)
                    {
                        index = node.pathID = (s.ReadByte());
                        if (index != (-1&0xff))
                            node[(PathType)j] = allPath[index];
                        node.pathlenght[j] = (s.ReadByte());
                    }
                }
            }
            return allPath[0];
        }
    }
}
