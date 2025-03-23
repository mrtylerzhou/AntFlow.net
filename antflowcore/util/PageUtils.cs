using antflowcore.constant.enus;
using antflowcore.dto;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.vo;

namespace antflowcore.util;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json; // Using System.Text.Json for JSON serialization


public static class PageUtils
{
    public static PageDto GetPageDto<T>(Page<T> page)
    {
        var pageDto = new PageDto
        {
            Page =(int) page.GetCurrent(),
            PageSize = (int)page.GetSize(),
            TotalCount = (int)page.GetTotal(),
            PageCount = (int)page.GetPages()
        };
        return pageDto;
    }

    public static int GetPages(int pageSize, int totalCount)
    {
        if (pageSize == 0)
        {
            return 0;
        }
        else
        {
            int pages = totalCount / pageSize;
            if (totalCount % pageSize != 0L)
            {
                ++pages;
            }

            return pages;
        }
    }

    public static Page<T> GetPageByPageDto<T>(PageDto pageDto)
    {
        return BuildPage<T>(pageDto);
    }

    public static Page<T> GetPageByPageDto<T>(PageDto pageDto, List<PageSortVo> pageSortVos)
    {
        var page = BuildPage<T>(pageDto);
        if (pageSortVos == null || !pageSortVos.Any())
        {
            return page;
        }

        SetOrderByField(page, pageDto, pageSortVos ?? new List<PageSortVo>());
        return page;
    }

    public static Page<T> GetPageByPageDto<T>(PageDto pageDto, SortedDictionary<string, SortTypeEnum> orderFieldMap)
    {
        var page = BuildPage<T>(pageDto);
        var pageSortVos = new List<PageSortVo>();
        if (orderFieldMap != null && orderFieldMap.Any())
        {
            foreach (var entry in orderFieldMap)
            {
                if (string.IsNullOrEmpty(pageDto.OrderColumn) && pageDto.OrderColumn.Equals(entry.Key))
                {
                    continue;
                }

                pageSortVos.Add(new PageSortVo { OrderField = entry.Key, SortTypeEnum = entry.Value });
            }
        }

        SetOrderByField(page, pageDto, pageSortVos);
        return page;
    }

    public static ResultAndPage<R> GetResultAndPage<T, R>(Page<T> page, Func<T, R> mapper)
    {
        return new ResultAndPage<R>(page.GetRecords().Select(mapper).ToList(), GetPageDto(page));
    }

    public static ResultAndPage<T> GetResultAndPage<T>(Page<T> page)
    {
        return new ResultAndPage<T>(page.GetRecords(), GetPageDto(page));
    }

    public static ResultAndPage<T> GetResultAndPage<T>(List<T> data, PageDto pageDto)
    {
        return new ResultAndPage<T>(data, pageDto);
    }

    public static ResultAndPage<T> GetResultAndPage<T>(Page<T> page, Dictionary<string, object> statistics)
    {
        return new ResultAndPage<T>(page.GetRecords(), GetPageDto(page), statistics);
    }

    public static ResultAndPage<T> GetResultAndPage<T>(Page<T> page, Dictionary<string, object> statistics,
        Dictionary<string, string> sortColumnMap)
    {
        return new ResultAndPage<T>(page.GetRecords(), GetPageDto(page), statistics, sortColumnMap);
    }



    public static PageDto GetPageDtoByVo(object obj)
    {
        var objJson = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<PageDto>(objJson);
    }

    private static Page<T> BuildPage<T>(PageDto pageDto)
    {
        var page = new Page<T>((pageDto.Page < 1 ? 1 : pageDto.Page),
            (pageDto.PageSize > 200 ? 200 : pageDto.PageSize) < 1 ? 1 : pageDto.PageSize);
        if (pageDto.PageSize == null) pageDto.PageSize = 10;
        if (pageDto.Page == null) pageDto.Page = 1;
        return page;
    }

    private static void SetOrderByField<T>(Page<T> page, PageDto pageDto, List<PageSortVo> pageSortVos)
    {
        List<PageSortVo> pageSortVoList = new List<PageSortVo>();
        pageSortVoList.AddRange(pageSortVos);
        if (!string.IsNullOrEmpty(pageDto.OrderColumn) && pageDto.OrderType.HasValue)
        {
            pageSortVoList.Add(new PageSortVo
            {
                OrderField = pageDto.OrderColumn,
                SortTypeEnum = SortTypeEnum.GetSortTypeEnumByCode(pageDto.OrderType.Value)
            });
        }

        if (pageSortVoList.Count == 0)
        {
            return;
        }

        //build the ordering fields

        for (int i = 0; i < pageSortVoList.Count; i++)
        {
            PageSortVo vo = pageSortVoList[i];
            OrderItem orderItem = new OrderItem();
            orderItem.SetColumn(vo.OrderField);
            orderItem.SetAsc(vo.SortTypeEnum.IsAsc);
            page.Orders().Add(orderItem);
        }
    }

    public static int GetPageCount(int total, int size)
    {
        return total % size == 0 ? total / size : total / size + 1;
    }

    public static List<T> GetPageList<T>(int page, int pageSize, int pageTotalCount, List<T> list)
    {
        if (page > pageTotalCount)
        {
            throw new AFBizException("999", "wrong page count info");
        }

        if (!page.Equals(pageTotalCount))
        {
            return list.GetRange((page - 1) * pageSize, pageSize);
        }

        return list.GetRange((page - 1) * pageSize, list.Count);
    }
}