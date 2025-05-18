
using FreeSql.Internal.Model;

namespace antflowcore.entity;

using System;
using System.Collections.Generic;
using System.Linq;

    public class Page<T>
    {
        public List<T> Records { get; set; } = new List<T>();
        public int Total = 0;
        public int Size = 10;
        public int Current = 1;
        public List<OrderItem> Orders = new List<OrderItem>();
        public string CountId { get; set; }
        public long? MaxLimit { get; set; }
        public bool SearchCount { get; set; }
        public Page() { }

        public Page(int current, int size) : this(current, size, 0) { }

        public Page(int current, int size, int total) : this(current, size, total, true) { }

        public Page(int current, int size, bool searchCount) : this(current, size, 0, searchCount) { }

        public Page(int current, int size, int total, bool searchCount)
        {
            if (current > 1L)
            {
                this.Current = current;
            }

            this.Size = size;
            this.Total = total;
            this.SearchCount = searchCount;
        }

        public bool HasPrevious() => this.Current > 1L;

        public bool HasNext() => this.Current < GetPages();

        private string[] MapOrderToArray(Func<OrderItem,bool> filter)
        {
           
            return this.Orders.Where(filter).Select(i => i.GetColumn()).ToArray();
        }

        private void RemoveOrder(Predicate<OrderItem> filter)
        {
            this.Orders.RemoveAll(filter);
        }

        public Page<T> AddOrder(params OrderItem[] items)
        {
            this.Orders.AddRange(items);
            return this;
        }

        public Page<T> AddOrder(List<OrderItem> items)
        {
            this.Orders.AddRange(items);
            return this;
        }
        



        public static Page<T> Of(int current, int size, int total, bool searchCount)
        {
            return new Page<T>(current, size, total, searchCount);
        }
        

        public Page<T> SetSearchCount(bool searchCount)
        {
            this.SearchCount = searchCount;
            return this;
        }
        
        
      public  int GetPages()
        {
            if (Size == 0)
            {
                return 0;
            }
            else
            {
                int pages =Total / Size;
                if (Total % Size != 0L)
                {
                    ++pages;
                }

                return pages;
            }
        }
      
        
      public  long Offset()
        {
            long current = Current;
            return current <= 1L ? 0L : Math.Max((current - 1L) * Size, 0L);
        }

        public BasePagingInfo ToPagingInfo()
        {
            BasePagingInfo info = new BasePagingInfo()
            {
                PageSize = (int)this.Size,
                PageNumber = (int)this.Current,
            };
            return info;
        }

        public Page<T> Of(ICollection<T> records, BasePagingInfo pagingInfo)
        {
            this.Records.AddRange(records);
            this.Total = (int)pagingInfo.Count;
            return this;
        }
    }


