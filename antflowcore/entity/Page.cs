using antflowcore.interf;

namespace antflowcore.entity;

using System;
using System.Collections.Generic;
using System.Linq;

    public class Page<T> : IPage<T>
    {
        private List<T> records = new List<T>();
        private long total = 0L;
        private long size = 10L;
        private long current = 1L;
        private List<OrderItem> orders = new List<OrderItem>();
        private bool optimizeCountSql = true;
        private bool searchCount = true;
        private bool optimizeJoinOfCountSql = true;
        private string countId;
        private long? maxLimit;

        public Page() { }

        public Page(long current, long size) : this(current, size, 0L) { }

        public Page(long current, long size, long total) : this(current, size, total, true) { }

        public Page(long current, long size, bool searchCount) : this(current, size, 0L, searchCount) { }

        public Page(long current, long size, long total, bool searchCount)
        {
            if (current > 1L)
            {
                this.current = current;
            }

            this.size = size;
            this.total = total;
            this.searchCount = searchCount;
        }

        public bool HasPrevious() => this.current > 1L;

        public bool HasNext() => this.current < GetPages();

        public List<T> GetRecords() => this.records;

        public IPage<T> SetRecords(List<T> records)
        {
            this.records = records;
            return this;
        }

        public long GetTotal() => this.total;

        public IPage<T> SetTotal(long total)
        {
            this.total = total;
            return this;
        }

        public long GetSize() => this.size;

        public IPage<T> SetSize(long size)
        {
            this.size = size;
            return this;
        }

        public long GetCurrent() => this.current;

        public IPage<T> SetCurrent(long current)
        {
            this.current = current;
            return this;
        }

        public string CountId() => this.countId;

        public long? MaxLimit() => this.maxLimit;

        private string[] MapOrderToArray(Func<OrderItem,bool> filter)
        {
           
            return this.orders.Where(filter).Select(i => i.GetColumn()).ToArray();
        }

        private void RemoveOrder(Predicate<OrderItem> filter)
        {
            this.orders.RemoveAll(filter);
        }

        public IPage<T> AddOrder(params OrderItem[] items)
        {
            this.orders.AddRange(items);
            return this;
        }

        public Page<T> AddOrder(List<OrderItem> items)
        {
            this.orders.AddRange(items);
            return this;
        }

        public List<OrderItem> Orders() => this.orders;

        public bool OptimizeCountSql() => this.optimizeCountSql;

        public static IPage<T> Of(long current, long size, long total, bool searchCount)
        {
            return new Page<T>(current, size, total, searchCount);
        }

        public bool OptimizeJoinOfCountSql() => this.optimizeJoinOfCountSql;

        public IPage<T> SetSearchCount(bool searchCount)
        {
            this.searchCount = searchCount;
            return this;
        }

        public IPage<T> SetOptimizeCountSql(bool optimizeCountSql)
        {
            this.optimizeCountSql = optimizeCountSql;
            return this;
        }

        public long GetPages()
        {
            return (long)Math.Ceiling((double)this.total / this.size);
        }

     

        public void SetOrders(List<OrderItem> orders)
        {
            this.orders = orders;
        }

        public void SetOptimizeJoinOfCountSql(bool optimizeJoinOfCountSql)
        {
            this.optimizeJoinOfCountSql = optimizeJoinOfCountSql;
        }

        public void SetCountId(string countId)
        {
            this.countId = countId;
        }

        public void SetMaxLimit(long? maxLimit)
        {
            this.maxLimit = maxLimit;
        }
    }


