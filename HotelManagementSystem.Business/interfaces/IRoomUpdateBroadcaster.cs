namespace HotelManagementSystem.Business.interfaces
{
    public interface IRoomUpdateBroadcaster
    {
        Task BroadcastRoomStatusAsync(int roomId, string roomNumber, string newStatus);
    }
}
