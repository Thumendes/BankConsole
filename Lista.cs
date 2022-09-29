using System;
using System.Reflection;
using System.Runtime.ConstrainedExecution;

namespace ListaTeste
{
    public class Node<T>
    {
        public T value;
        public Node<T>? next;

        public Node(T value)
        {
            this.value = value;
        }
    }

    public class Lista<T>
    {
        public Node<T>? head;

        public Lista<T> Add(T value)
        {
            var node = new Node<T>(value);
            if (head == null)
            {
                head = node;
            }
            else
            {
                Node<T> aux = head;
                while (aux.next != null)
                {
                    aux = aux.next;
                }

                aux.next = node;
            }

            return this;
        }

        public void Remove(T value)
        {
            Node<T>? aux = head;
            Node<T>? prev = head;
            while (aux != null && aux.value != null && prev != null)
            {
                if (aux.value.Equals(value)) prev.next = aux.next;
                prev = aux;
                aux = aux.next;
            }
        }

        public delegate void ForEachCallback(T value, int index);
        public void ForEach(ForEachCallback Callback)
        {
            Node<T>? aux = head;
            int index = 0;
            while (aux != null)
            {
                Callback(aux.value, index);
                aux = aux.next;
                index++;
            }
        }

        public delegate V MapCallback<V>(T value, int index);
        public Lista<V> Map<V>(MapCallback<V> Callback)
        {
            var newList = new Lista<V>();
            ForEach((value, index) => newList.Add(Callback(value, index)));
            return newList;
        }

        public delegate bool FilterCallback(T value, int index);
        public Lista<T> Filter(FilterCallback Callback)
        {
            var newList = new Lista<T>();
            ForEach((value, index) =>
            {
                if (Callback(value, index)) newList.Add(value);
            });
            return newList;
        }

        public delegate bool FindCallback(T value, int index);
        public T? Find(FindCallback Callback)
        {
            Node<T>? aux = head;
            int index = 0;
            while (aux != null)
            {
                if (Callback(aux.value, index)) return aux.value;
                aux = aux.next;
                index++;
            }
            return default(T);
        }

        public delegate bool FindIndexCallback(T value, int index);
        public int FindIndex(FindIndexCallback Callback)
        {
            Node<T>? aux = head;
            int index = 0;
            while (aux != null)
            {
                if (Callback(aux.value, index)) return index;
                aux = aux.next;
                index++;
            }
            return -1;
        }

        public delegate V ReduceCallback<V>(V acc, T value, int index);
        public V Reduce<V>(ReduceCallback<V> Callback, V value)
        {
            Node<T>? aux = head;
            int index = 0;
            while (aux != null)
            {
                value = Callback(value, aux.value, index);
                aux = aux.next;
                index++;
            }
            return value;
        }

        public T? At(int index)
        {
            Node<T>? aux = head;
            int currIndex = 0;
            while (aux != null)
            {
                if (currIndex == index) return aux.value;
                aux = aux.next;
                currIndex++;
            }
            return default(T);
        }

        public int Length()
        {
            var count = 0;
            ForEach((value, index) => count++);
            return count;
        }

        public override string ToString()
        {
            string text = "{";
            int size = Length();
            ForEach((value, index) =>
            {
                string additional = index == size - 1 ? $"{value}}}" : $"{value}, ";
                text += additional;
            });
            return text;
        }
    }
}