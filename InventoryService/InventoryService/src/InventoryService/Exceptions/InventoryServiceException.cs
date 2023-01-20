namespace InventoryService.Exceptions
{
    [Serializable]
    public class InventoryServiceException: Exception
    {
        public InventoryServiceException()
        {
        }

        public InventoryServiceException(string message) : base(message)
        {
        }

        public InventoryServiceException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
