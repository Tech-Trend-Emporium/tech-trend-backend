using Data.Entities;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="Session"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface ISessionRepository : IEfRepository<Session>
    {
        /// <summary>
        /// Retrieves a list of active <see cref="Session"/> entities associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose active sessions are being retrieved.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a list of active <see cref="Session"/> entities for the specified user.
        /// </returns>
        Task<List<Session>> GetActiveByUserAsync(int userId, CancellationToken ct = default);
    }
}
