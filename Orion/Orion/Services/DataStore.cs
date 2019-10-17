using Orion.Domain;
using Orion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orion.Services
{
    public class DataStore : IDataStore<Item>
    {
        private readonly List<Item> _items;
        private User _user = new User();

        public DataStore()
        {
            _items = new List<Item>()
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "First 1 item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Second 2 item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." }
            };
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> SetUser(User user)
        {
            _user = user;

            return await Task.FromResult(true);
        }

        public async Task<User> GetUser()
        {
            return await Task.FromResult(_user);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = _items.FirstOrDefault(arg => arg.Id == item.Id);
            _items.Remove(oldItem);
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = _items.FirstOrDefault(arg => arg.Id == id);
            _items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(_items);
        }
    }
}