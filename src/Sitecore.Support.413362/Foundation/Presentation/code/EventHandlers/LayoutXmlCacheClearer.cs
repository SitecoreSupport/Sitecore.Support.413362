// Decompiled with JetBrains decompiler
// Type: Sitecore.XA.Foundation.Presentation.EventHandlers.LayoutXmlCacheClearer
// Assembly: Sitecore.XA.Foundation.Presentation, Version=3.6.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B8579EB5-F4CA-4A0B-A13F-D3C2BA93FE8F
// Assembly location: C:\inetpub\wwwroot\sxa160sc901CD.local\bin\Sitecore.XA.Foundation.Presentation.dll

using Microsoft.Extensions.DependencyInjection;
using Sitecore.Caching;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.XA.Foundation.Caching.EventHandlers;
using Sitecore.XA.Foundation.Multisite;
using Sitecore.XA.Foundation.Multisite.Services;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Support.XA.Foundation.Presentation.EventHandlers
{
    public class LayoutXmlCacheClearer : Sitecore.XA.Foundation.Presentation.EventHandlers.LayoutXmlCacheClearer
    {
        protected override void ClearCache(Item item, bool clearPerDatabase = true)
        {

            bool _isCarouselItem = item.InheritsFrom(new ID("{ADD22F05-6B4C-4344-95AD-9A1A9BA6A216}"));
            if (!_isCarouselItem && (!item.Paths.IsContentItem || !item.InheritsFrom(Sitecore.XA.Foundation.Presentation.Templates.PartialDesign.ID) && !item.InheritsFrom(Sitecore.XA.Foundation.Presentation.Templates.Design.ID) && !item.InheritsFrom(Sitecore.XA.Foundation.Multisite.Templates.Page.ID)))
                return;
            Item siteItem1 = ServiceLocator.ServiceProvider.GetService<IMultisiteContext>().GetSiteItem(item);
            if (siteItem1 != null)
            {
                Log.Debug("LayoutXmlCacheClearer - clearing cache", (object)this);
                ICache _cache = CacheManager.FindCacheByName("SXA[LayoutXml]");
                var _cacheKey = clearPerDatabase ? this.GetKey(siteItem1.ID, item.Database.Name) : this.GetKey(siteItem1.ID, (string)null);

                if (_cache != null)
                {                  
                    _cache.RemovePrefix(_cacheKey);                  
                }
                
                List<Item> list = item.GetClones().ToList<Item>();

                if (list.Any<Item>())
                {
                    IDelegatedAreaService service = ServiceLocator.ServiceProvider.GetService<IDelegatedAreaService>();
                    foreach (Item obj in list)
                    {
                        if (service.CheckForDelegatedArea(obj))
                        {
                            Item siteItem2 = ServiceLocator.ServiceProvider.GetService<IMultisiteContext>().GetSiteItem(item);
                            CacheManager.FindCacheByName("SXA[LayoutXml]")?.RemovePrefix(clearPerDatabase ? this.GetKey(siteItem2.ID, item.Database.Name) : this.GetKey(siteItem2.ID, (string)null));
                        }
                    }
                }
            }
            Log.Debug("LayoutXmlCacheClearer - done", (object)this);
        }
                
        protected override void OnPublish(object sender, string dbName, ID itemId)
        {
            Item item1;
            Database database = ServiceProviderServiceExtensions.GetService<IDatabaseRepository>(ServiceLocator.ServiceProvider).GetDatabase(dbName);
            if (database != null)
            {
                item1 = database.GetItem(itemId);
            }
            else
            {
                Database local1 = database;
                item1 = null;
            }
            Item item = item1;
            if (item != null)
            {
                this.ClearCache(item, false);
            }
        }
    }
}
