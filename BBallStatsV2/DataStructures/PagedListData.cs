namespace BBallStatsV2.DataStructures
{
    public static class PagedListData
    {
        private static int MinPageIndex = 1;
        private static int MinPageSize = 1;
        private static int MaxPageSize = 30;

        public static int FilteredPageSize(int pageSize)
        {
            return Math.Clamp(pageSize, MinPageSize, MaxPageSize);
        }
        public static int FilteredPageIndex(int pageIndex)
        {
            return pageIndex > MinPageIndex ? pageIndex : MinPageIndex;
        }
    }
}
