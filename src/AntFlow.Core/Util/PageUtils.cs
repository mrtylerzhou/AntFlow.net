using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Vo;
using System.Text.Json;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Util;

public static class PageUtils
{
    public static PageDto GetPageDto<T>(Page<T> page)
    {
        PageDto? pageDto = new()
        {
            Page = page.Current, PageSize = page.Size, TotalCount = page.Total, PageCount = page.GetPages()
        };
        return pageDto;
    }

    public static int GetPages(int pageSize, int totalCount)
    {
        if (pageSize == 0)
        {
            return 0;
        }

        int pages = totalCount / pageSize;
        if (totalCount % pageSize != 0L)
        {
            ++pages;
        }

        return pages;
    }

    public static Page<T> GetPageByPageDto<T>(PageDto pageDto)
    {
        return BuildPage<T>(pageDto);
    }

    public static Page<T> GetPageByPageDto<T>(PageDto pageDto, List<PageSortVo> pageSortVos)
    {
        Page<T>? page = BuildPage<T>(pageDto);
        if (pageSortVos == null || !pageSortVos.Any())
        {
            return page;
        }

        SetOrderByField(page, pageDto, pageSortVos ?? new List<PageSortVo>());
        return page;
    }

    public static Page<T> GetPageByPageDto<T>(PageDto pageDto, SortedDictionary<string, SortTypeEnum> orderFieldMap)
    {
        Page<T>? page = BuildPage<T>(pageDto);
        List<PageSortVo>? pageSortVos = new();
        if (orderFieldMap != null && orderFieldMap.Any())
        {
            foreach (KeyValuePair<string, SortTypeEnum> entry in orderFieldMap)
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
        return new ResultAndPage<R>(page.Records.Select(mapper).ToList(), GetPageDto(page));
    }

    public static ResultAndPage<T> GetResultAndPage<T>(Page<T> page)
    {
        return new ResultAndPage<T>(page.Records, GetPageDto(page));
    }

    public static ResultAndPage<T> GetResultAndPage<T>(List<T> data, PageDto pageDto)
    {
        return new ResultAndPage<T>(data, pageDto);
    }

    public static ResultAndPage<T> GetResultAndPage<T>(Page<T> page, Dictionary<string, object> statistics)
    {
        return new ResultAndPage<T>(page.Records, GetPageDto(page), statistics);
    }

    public static ResultAndPage<T> GetResultAndPage<T>(Page<T> page, Dictionary<string, object> statistics,
        Dictionary<string, string> sortColumnMap)
    {
        return new ResultAndPage<T>(page.Records, GetPageDto(page), statistics, sortColumnMap);
    }


    public static PageDto GetPageDtoByVo(object obj)
    {
        string? objJson = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<PageDto>(objJson);
    }

    private static Page<T> BuildPage<T>(PageDto pageDto)
    {
        Page<T>? page = new(pageDto.Page < 1 ? 1 : pageDto.Page,
            (pageDto.PageSize > 200 ? 200 : pageDto.PageSize) < 1 ? 1 : pageDto.PageSize);
        if (pageDto.PageSize == null)
        {
            pageDto.PageSize = 10;
        }

        if (pageDto.Page == null)
        {
            pageDto.Page = 1;
        }

        return page;
    }

    private static void SetOrderByField<T>(Page<T> page, PageDto pageDto, List<PageSortVo> pageSortVos)
    {
        List<PageSortVo> pageSortVoList = new();
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
            OrderItem orderItem = new();
            orderItem.SetColumn(vo.OrderField);
            orderItem.SetAsc(vo.SortTypeEnum.IsAsc);
            page.Orders.Add(orderItem);
        }
    }

    public static int GetPageCount(int total, int size)
    {
        return total % size == 0 ? total / size : (total / size) + 1;
    }

    public static List<T> GetPageList<T>(int page, int pageSize, int pageTotalCount, List<T> list)
    {
        if (page > pageTotalCount)
        {
            throw new AntFlowException.AFBizException("999", "wrong page count info");
        }

        if (!page.Equals(pageTotalCount))
        {
            return list.GetRange((page - 1) * pageSize, pageSize);
        }

        return list.GetRange((page - 1) * pageSize, list.Count);
    }
}