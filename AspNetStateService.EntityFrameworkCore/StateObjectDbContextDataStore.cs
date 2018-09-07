using System;
using System.Threading.Tasks;

using AspNetStateService.Core;

using Cogito.Autofac;

using Microsoft.EntityFrameworkCore;

namespace AspNetStateService.EntityFrameworkCore
{

    /// <summary>
    /// Implements a <see cref="IStateObjectDataStore"/> using Entity Framework Core.
    /// </summary>
    [RegisterAs(typeof(IStateObjectDataStore))]
    public class StateObjectDbContextDataStore : IStateObjectDataStore
    {

        readonly IStateObjectDbContextProvider dbContextProvider;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public StateObjectDbContextDataStore(IStateObjectDbContextProvider dbContextProvider)
        {
            this.dbContextProvider = dbContextProvider ?? throw new ArgumentNullException(nameof(dbContextProvider));
        }

        public async Task<(byte[] data, uint? extraFlags, TimeSpan? timeout, DateTime? touch)> GetDataAsync(string id)
        {
            using (var db = await dbContextProvider.CreateDbContextAsync())
            {
                var o = await db.State.Include(i => i.Data).FirstOrDefaultAsync(i => i.Id == id);
                return (o?.Data?.Value, o?.ExtraFlags, o?.Timeout, o?.Touched);
            }
        }

        public async Task<(uint? cookie, DateTime? time)> GetLockAsync(string id)
        {
            using (var db = await dbContextProvider.CreateDbContextAsync())
            {
                var o = await db.FindAsync<State>(id);
                return (o?.LockCookie, o?.LockTime);
            }
        }

        public async Task RemoveDataAsync(string id)
        {
            using (var db = await dbContextProvider.CreateDbContextAsync())
            {
                var o = await db.State.Include(i => i.Data).FirstOrDefaultAsync(i => i.Id == id);
                if (o != null)
                {
                    o.Data = null;
                    o.ExtraFlags = null;
                    o.Timeout = null;
                    o.Touched = null;
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task RemoveLockAsync(string id)
        {
            using (var db = await dbContextProvider.CreateDbContextAsync())
            {
                var o = await db.FindAsync<State>(id);
                if (o != null)
                {
                    o.LockCookie = null;
                    o.LockTime = null;
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task SetDataAsync(string id, byte[] data, uint? extraFlags, TimeSpan? timeout)
        {
            using (var db = await dbContextProvider.CreateDbContextAsync())
            {
                var o = await db.State.Include(i => i.Data).FirstOrDefaultAsync(i => i.Id == id);
                if (o == null)
                    db.State.Add(o = new State() { Id = id } );

                if (o.Data == null)
                    o.Data = new StateData() { Id = o.Id, State = o };

                o.Data.Value = data;
                o.ExtraFlags = extraFlags;
                o.Timeout = timeout;
                o.Touched = DateTime.Now;

                await db.SaveChangesAsync();
            }
        }

        public async Task SetFlagAsync(string id, uint? extraFlags)
        {
            using (var db = await dbContextProvider.CreateDbContextAsync())
            {
                var o = await db.FindAsync<State>(id);
                if (o == null)
                    db.State.Add(o = new State() { Id = id });

                o.ExtraFlags = extraFlags;

                await db.SaveChangesAsync();
            }
        }

        public async Task SetLockAsync(string id, uint cookie, DateTime time)
        {
            using (var db = await dbContextProvider.CreateDbContextAsync())
            {
                var o = await db.FindAsync<State>(id);
                if (o == null)
                    db.State.Add(o = new State() { Id = id });

                o.LockCookie = cookie;
                o.LockTime = time;

                await db.SaveChangesAsync();
            }
        }

        public async Task SetTimeoutAsync(string id, TimeSpan? timeout)
        {
            using (var db = await dbContextProvider.CreateDbContextAsync())
            {
                var o = await db.FindAsync<State>(id);
                if (o == null)
                    db.State.Add(o = new State() { Id = id });

                o.Timeout = timeout;

                await db.SaveChangesAsync();
            }
        }

    }

}
