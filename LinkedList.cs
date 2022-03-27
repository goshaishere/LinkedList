using System;
using System.Collections.Generic;
using System.IO;

namespace RandList
{
    public class ListNode
    {
        public string Data;
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random;
    }

    public class ListRand
    {
        ListNode head;
        ListNode tail;
        int count;

        public int getCount()
        {
            return count;
        }
        public void Serialize(FileStream s)
        {
            List<ListNode> array = new List<ListNode>();
            ListNode current = head;
            while (current != null)
            {
                array.Add(current);
                current = current.Next;
            }
            using (StreamWriter w = new StreamWriter(s))
                foreach (ListNode n in array)
                    w.WriteLine(n.Data.ToString() + ":" + array.IndexOf(n.Random).ToString());
        }

        public void Deserialize(FileStream s)
        {
            List<ListNode> arr = new List<ListNode>();
            ListNode current = new ListNode();
            count = 0;
            head = current;
            string line;

            try
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Equals(""))
                        {
                            count++;
                            current.Data = line;
                            ListNode next = new ListNode();
                            current.Next = next;
                            arr.Add(current);
                            next.Previous = current;
                            current = next;
                        }
                    }
                }
                //declare Tail
                tail = current.Previous;
                tail.Next = null;

                //return refs to Random nodes and restore Data
                foreach (ListNode n in arr)
                {
                    n.Random = arr[Convert.ToInt32(n.Data.Split(':')[1])];
                    n.Data = n.Data.Split(':')[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Не удалось обработать файл данных, возможно, он поврежден, подробности:");
                Console.WriteLine(e.Message);
                Console.WriteLine("Press Enter to exit.");
                Console.Read();
                Environment.Exit(0);
            }
        }

        public void Add(string data)
        {
            ListNode node = new ListNode();
            node.Data = data;
            if (head == null)
                head = node;
            else
            {
                tail.Next = node;
                node.Previous = tail;
            }
            tail = node;
            count++;
        }

        public void addRandomNodes()
        {
            Random rand = new Random();
            ListNode current4iter = head;
            int i = 0;
            while (i < count)
            {
                ListNode current = head;

                int randNum = rand.Next(0, count);
                int j = 0;
                while (j <= randNum)
                {
                    if (j == randNum)
                    {
                        current4iter.Random = current;
                    }
                    current = current.Next;
                    j++;
                }
                i++;
                current4iter = current4iter.Next;
            }
        }

        public void PrintListDeps()
        {
            ListNode current = head;
            int i = 0;
            while (i < count)
            {
                Console.WriteLine("New node");
                Console.WriteLine(current.Data + " - node data");
                if (current.Next != null)
                {
                    Console.WriteLine(current.Next.Data + " - node next");
                }
                if (current.Previous != null)
                {
                    Console.WriteLine(current.Previous.Data + " - node prev");
                }

                if (current.Random != null)
                {
                    Console.WriteLine(current.Random.Data + " - node rndm");
                }
                i++;
                current = current.Next;
            }
        }
    }

    class Program
    {
        static Random rand = new Random();
        static void Main(string[] args)
        {
            ListRand linkedList = new ListRand();
            linkedList.Add("Bob");
            linkedList.Add("Bill");
            linkedList.Add("Will");
            linkedList.Add("Arsen");
            linkedList.PrintListDeps();
            Console.WriteLine("_______________");
            linkedList.addRandomNodes();
            linkedList.PrintListDeps();

            FileStream fs = new FileStream("dat.dat", FileMode.OpenOrCreate);
            linkedList.Serialize(fs);

            Console.WriteLine("Serialize OK");

            ListRand second = new ListRand();
            FileStream fs2 = new FileStream("dat.dat", FileMode.Open);
            second.Deserialize(fs2);
            second.PrintListDeps();

            Console.WriteLine("Deserialize OK");
        }
    }
}