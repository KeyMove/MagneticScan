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
        public object Tag;

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
        Pen norpen = new Pen(Color.Gray,5);
        Brush norBrush = Brushes.Gray;
        Brush deadBrush = Brushes.Black;
        Pen newpen = new Pen(Color.SkyBlue, 3);
        Brush targetBrush = Brushes.Red;
        Pen PathPen = new Pen(Color.GreenYellow, 1);
        Brush CarBrush = Brushes.Orange;

        List<PathNode> TargetPath = new List<PathNode>();
        List<PathType> TargetDirList = new List<PathType>();
        List<PathNode> NodeList=new List<PathNode>();
        List<PathNode> EndPathList;
        PathDir findDir=new PathDir();
        Font strFont = new Font("宋体", 9);    
        int xpos, ypos;
        int width=10;
        List<PathNode> allPath = new List<PathNode>();

        int nodescount = 0;
        PathNode nodes;

        PathNode TargetNode;
        PathNode lastNode;
        PathType lastPath;
        bool TurnBack = false;
        bool isOver = false;
        PathDir LastDir=new PathDir();

        PathDir LastPathDirValue = new PathDir();
        public PathDir LastPathDir
        {
            get { return LastPathDirValue.clone(); }
            set
            {
                if (value != null)
                    LastPathDirValue = value.clone();
            }
        }

        public bool WayLenght { get; private set; }

        Stack<PathNode> searchStack = new Stack<PathNode>();
        private int lastlenght;

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


        //public List<PathNode> getAllNode(List<PathNode> list = null)
        //{
        //    List<PathNode> nodes = list;
        //    if (nodes == null)
        //    {
        //        nodes = new List<PathNode>();
        //    }
        //    for (int i = 0; i < paths.Length; i++)
        //    {
        //        if (this.paths[i] != null)
        //        {
        //            if (nodes.Contains(this.paths[i])) continue;
        //            nodes.Add(this.paths[i]);
        //            this.paths[i].getAllNode(nodes);
        //        }
        //    }
        //    return nodes;
        //}
        List<PathNode> DrawPathList(int x,int y,PathNode node,List<PathNode> pathlist=null, List<Point> ps=null)
        {
            List<PathNode> nodes = pathlist;
            List<Point> Points=ps;
            bool f = false;
            if (nodes == null)
            {
                f = true;
                Points = new List<Point>();
                nodes = new List<PathNode>();
            }
            int ax=0, ay=0;
            int w;
            for (int i = 0; i < node.paths.Length; i++)
            {
                if (node.paths[i] != null)
                {
                    if (nodes.Contains(node.paths[i])) continue;
                    if ((w = node.pathlenght[i]) == 0) w = width;
                    switch ((PathType)i)
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

                    if (!node.paths[i].isDead)
                    {
                        Draw.DrawLine(norpen, x, y, x + ax, y + ay);
                        Draw.FillEllipse(norBrush, new Rectangle(x + ax - 2, y + ay - 2, 4, 4));
                        //if (lastNode == node.paths[i])
                        //    Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                    }
                    else if (node.paths[i].isNew)
                    {
                        Draw.DrawLine(newpen, x, y, x + ax, y + ay);
                        node.paths[i].x = x + ax;
                        node.paths[i].y = y + ay;
                    }
                    else
                    {
                        Draw.DrawLine(norpen, x, y, x + ax, y + ay);
                        //Draw.DrawEllipse(deadpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                        //if (lastNode == node.paths[i])
                        //    Draw.DrawEllipse(targetpen, new Rectangle(x + ax - 5, y + ay - 5, 10, 10));
                        Draw.DrawString(node.paths[i].pathName, strFont, Brushes.Black, x + ax + 5, y + ay + 5);
                        node.paths[i].x = x + ax;
                        node.paths[i].y = y + ay;
                        Points.Add(new Point(node.paths[i].x, node.paths[i].y));
                    }

                    nodes.Add(node.paths[i]);
                    DrawPathList(x + ax, y + ay, node.paths[i], nodes, Points);
                }
            }
            if (f)
            {
                foreach(Point p in Points)
                {
                    Draw.FillRectangle(deadBrush,p.X-8,p.Y-8,16,16);
                }
                Draw.FillEllipse(targetBrush, new Rectangle(lastNode.x - 5, lastNode.y - 5, 10, 10));
            }
            return nodes;
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
            DrawPathList(xpos, ypos, nodes);
            DrawTargetPath();
            updateCar(lastlenght);
            //DrawPath(xpos,ypos,nodes);
            //findDir.Rotate(PathType.Back);
            //DrawPath(nodes[PathType.Forward].x, nodes[PathType.Forward].y, nodes[PathType.Forward],PathType.Back);
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


        public List<PathNode> getLastTargetPath()
        {
            return TargetPath;
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
            TargetPath.Clear();
            TargetNode = nodes = new PathNode(nodescount++);
            searchStack.Clear();
            searchStack.Push(nodes);
            nodes[PathType.Forward] = new PathNode(nodescount++);
            nodes[PathType.Forward][PathType.Back] = nodes;
            lastNode = nodes[PathType.Forward];
            LastDir.Reset();
            TurnBack = false;
            WayLenght = true;
        }


        public void setCarNode(PathNode d,PathNode s=null)
        {

            if (s != null)
                TargetNode = s;
            else
                TargetNode = lastNode;
            lastNode = d;
            LastDir.Reset();
            PathType pt = TargetNode[d];
            //LastDir.Rotate(PathType.Back);
            LastDir.Rotate(pt);
        }

        void updateCar(int w)
        {
            int angle=0;
            int ax = 0, ay = 0;
            switch (LastDir.Forward)
            {
                case PathType.Forward:
                    ax = 0;
                    ay = -w;
                    angle = 0;
                    break;
                case PathType.Left:
                    ax = -w;
                    ay = 0;
                    angle = -90;
                    break;
                case PathType.Right:
                    ax = w;
                    ay = 0;
                    angle = 90;
                    break;
                case PathType.Back:
                    ax = 0;
                    ay = w;
                    angle = 180;
                    break;
            }
            drawCar(TargetNode.x+ax, TargetNode.y+ay, angle);
        }

        void drawCar(int x,int y,int angle)
        {
            Draw.FillEllipse(CarBrush, new Rectangle(x - 8, y - 8, 16, 16));
            Point[] ps = new Point[4];
            double dy = Math.Sin((angle - 180) * Math.PI / 180) * 8 + y;
            double dx = Math.Cos((angle - 180) * Math.PI / 180) * 8 + x;
            ps[0].X = (int)dx;
            ps[0].Y = (int)dy;
            dy = Math.Sin((angle - 90) * Math.PI / 180) * 14 + y;
            dx = Math.Cos((angle - 90) * Math.PI / 180) * 14 + x;
            ps[1].X = (int)dx;
            ps[1].Y = (int)dy;
            dy = Math.Sin((angle) * Math.PI / 180) * 8 + y;
            dx = Math.Cos((angle) * Math.PI / 180) * 8 + x;
            ps[2].X = (int)dx;
            ps[2].Y = (int)dy;

            ps[3].X = x;
            ps[3].Y = y;
            Draw.FillPolygon(CarBrush, ps);
            Draw.FillEllipse(Brushes.White, new Rectangle(x - 3, y - 3, 6, 6));
        }

        public void SetLastPathLenght(int lenght)
        {
            if(WayLenght)
            { lastNode.pathlenght[(int)LastDir.Back]=TargetNode.pathlenght[(int)LastDir.Forward] = lenght; }
            lastlenght = lenght> lastNode.pathlenght[(int)LastDir.Back]? lastNode.pathlenght[(int)LastDir.Back]:lenght;
        }

        public int GetLastPathLenght()
        {
            return lastNode.pathlenght[(int)LastDir.Back];
        }

        public int getlastTargetLenght()
        {
            return TargetNode.pathlenght[(int)TargetNode[lastNode]];
        }

        public void SetLastPathTag(object obj)
        {
            lastNode.Tag = obj;
        }

        public int SearchCheck(int lastPathSelect)
        {
            int PathOut = -1;
            WayLenght = false;
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
                    TargetNode = lastNode;
                    lastNode = searchStack.Pop();
                    PathOut = (int)PathType.Back;
                    LastDir.Rotate(PathType.Back);
                    TurnBack = true;
                }
                else
                {
                    WayLenght = true;
                    searchStack.Push(lastNode);
                    lastPath = LastDir[(PathType)PathOut];
                    LastDir.Rotate((PathType)PathOut);
                    lastNode[lastPath][LastDir.Back] = lastNode;
                    TargetNode = lastNode;
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
                            lastlenght = 0;
                            TargetNode = nodes[PathType.Forward];
                            return -1;
                        }
                        if (lastNode[PathType.Back].isDead)
                        {
                            searchStack.Push(lastNode);
                            PathOut = (int)LastDir.getDir(lastNode[lastNode[PathType.Back]]);
                            LastDir.Rotate((PathType)PathOut);
                            TargetNode = lastNode;
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
                    WayLenght = true;
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
