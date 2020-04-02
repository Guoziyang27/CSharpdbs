namespace BookManage
{
    public static class ApiResponse
    {
        public static object Success(object o)
        {
            return new {status = "success", data = o};
        }

        public static object Error(string errorMsg)
        {
            return new {status = "error", errorMsg};
        }
    }
}