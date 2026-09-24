using Nop.Core;
using Nop.Core.Domain.Menus;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Menus;

/// <summary>
/// Menu service
/// </summary>
public partial class MenuService : IMenuService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly IRepository<Menu> _menuRepository;
    protected readonly IRepository<MenuItem> _menuItemRepository;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public MenuService(IAclService aclService,
        IRepository<Menu> menuRepository,
        IRepository<MenuItem> menuItemRepository,
        IStoreMappingService storeMappingService,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering MenuService(aclService={AclService},menuRepository={MenuRepository},menuItemRepository={MenuItemRepository},storeMappingService=<redacted>,workContext={WorkContext})", aclService, menuRepository, menuItemRepository, workContext);
        _aclService = aclService;
        _menuRepository = menuRepository;
		_logger.LogDebug("MenuService(aclService={AclService},menuRepository={MenuRepository},menuItemRepository={MenuItemRepository},storeMappingService=<redacted>,workContext={WorkContext}): _menuRepository → {_menuRepository}", aclService, menuRepository, menuItemRepository, workContext, _menuRepository);
        _menuItemRepository = menuItemRepository;
		_logger.LogDebug("MenuService(aclService={AclService},menuRepository={MenuRepository},menuItemRepository={MenuItemRepository},storeMappingService=<redacted>,workContext={WorkContext}): _menuItemRepository → {_menuItemRepository}", aclService, menuRepository, menuItemRepository, workContext, _menuItemRepository);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("MenuService(aclService={AclService},menuRepository={MenuRepository},menuItemRepository={MenuItemRepository},storeMappingService=<redacted>,workContext={WorkContext}): _storeMappingService → {_storeMappingService}", aclService, menuRepository, menuItemRepository, workContext, _storeMappingService);
        _workContext = workContext;
		_logger.LogDebug("MenuService(aclService={AclService},menuRepository={MenuRepository},menuItemRepository={MenuItemRepository},storeMappingService=<redacted>,workContext={WorkContext}): _workContext → {_workContext}", aclService, menuRepository, menuItemRepository, workContext, _workContext);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Sort menu items for tree representation
    /// </summary>
    /// <param name="menuItemsByParentId">Menu items for sort</param>
    /// <param name="parentId">Parent menu item identifier</param>
    /// <param name="ignoreMenuItemWithoutExistingParent">A value indicating whether menu items without parent menu item in provided list (source) should be ignored</param>
    /// <returns>
    /// An enumerable containing the sorted menu items
    /// </returns>
    protected virtual IEnumerable<MenuItem> SortMenuItemsForTree(
        ILookup<int, MenuItem> menuItemsByParentId,
        int parentId = 0,
        bool ignoreMenuItemWithoutExistingParent = false)
    {
		_logger.LogError(">>> Entering SortMenuItemsForTree(menuItemsByParentId={MenuItemsByParentId},parentId={ParentId},ignoreMenuItemWithoutExistingParent={IgnoreMenuItemWithoutExistingParent})", menuItemsByParentId, parentId, ignoreMenuItemWithoutExistingParent);
        ArgumentNullException.ThrowIfNull(menuItemsByParentId);

        var remaining = parentId > 0
            ? new HashSet<int>(0)
            : menuItemsByParentId.Select(g => g.Key).ToHashSet();
        remaining.Remove(parentId);

        foreach (var item in menuItemsByParentId[parentId].OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id))
        {
            yield return item;

            remaining.Remove(item.Id);

            foreach (var subItem in SortMenuItemsForTree(menuItemsByParentId, item.Id, true))
            {
                yield return subItem;
                remaining.Remove(subItem.Id);
            }
        }

        if (ignoreMenuItemWithoutExistingParent)
            yield break;

        //find menu items without parent in provided menu item source and return them
        var orphans = remaining
            .SelectMany(id => menuItemsByParentId[id])
            .OrderBy(c => c.ParentId)
            .ThenBy(c => c.DisplayOrder)
            .ThenBy(c => c.Id);

        foreach (var orphan in orphans)
            yield return orphan;
			_logger.LogWarning("<<< Exiting SortMenuItemsForTree(menuItemsByParentId={MenuItemsByParentId},parentId={ParentId},ignoreMenuItemWithoutExistingParent={IgnoreMenuItemWithoutExistingParent})", menuItemsByParentId, parentId, ignoreMenuItemWithoutExistingParent);
    }

    #endregion

    #region Methods

    #region Menus

    /// <summary>
    /// Deletes a menu
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task DeleteMenuAsync(Menu menu)
    {
		_logger.LogError(">>> Entering DeleteMenuAsync(menu={Menu})", menu);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeleteMenuAsync(menu)={Menu}: external service call took {Arg1} ms", menu, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteMenuAsync(menu={Menu})", menu);
        return _menuRepository.DeleteAsync(menu);
    }

    /// <summary>
    /// Gets all menus
    /// </summary>
    /// <returns>
    /// <param name="menuType">Menu type</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="showHidden">A value indicating whether to load hidden records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// A task that represents the asynchronous operation
    /// The task result contains the menus
    /// </returns>
    public virtual async Task<IPagedList<Menu>> GetAllMenusAsync(
        MenuType? menuType = null,
        int storeId = 0,
        bool showHidden = false,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetAllMenusAsync(menuType={MenuType},storeId={StoreId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", menuType, storeId, showHidden, pageIndex, pageSize);
        return await _menuRepository.GetAllPagedAsync(async query =>
        {
			_logger.LogWarning("<<< Exiting GetAllMenusAsync(menuType={MenuType},storeId={StoreId},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", menuType, storeId, showHidden, pageIndex, pageSize);
            if (menuType is not null)
                query = query.Where(m => m.MenuTypeId == (int)menuType);
				_logger.LogError("GetAllMenusAsync(menuType,storeId,showHidden,pageIndex,pageSize)={MenuType},{StoreId},{ShowHidden},{PageIndex},{PageSize}: find query executing", menuType, storeId, showHidden, pageIndex, pageSize);

            if (!showHidden)
            {
                query = query.Where(m => m.Published);

                //apply ACL constraints
                var customer = await _workContext.GetCurrentCustomerAsync();
                query = await _aclService.ApplyAcl(query, customer);
            }

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            return query
                .OrderBy(m => m.MenuTypeId)
                .ThenBy(m => m.DisplayOrder)
                .ThenBy(m => m.Id);

        }, pageIndex, pageSize, includeDeleted: false);
    }

    /// <summary>
    /// Gets a menu by identifier
    /// </summary>
    /// <param name="menuId">The menu identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a menu
    /// </returns>
    public virtual Task<Menu> GetMenuByIdAsync(int menuId)
    {
		_logger.LogError(">>> Entering GetMenuByIdAsync(menuId={MenuId})", menuId);
		_logger.LogError("GetMenuByIdAsync(menuId)={MenuId}: find query executing", menuId);
    _logger.LogWarning("<<< Exiting GetMenuByIdAsync(menuId={MenuId})", menuId);
        return _menuRepository.GetByIdAsync(menuId, cache => default, includeDeleted: false);
    }

    /// <summary>
    /// Insert a menu
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task InsertMenuAsync(Menu menu)
    {
		_logger.LogError(">>> Entering InsertMenuAsync(menu={Menu})", menu);
        _logger.LogWarning("<<< Exiting InsertMenuAsync(menu={Menu})", menu);
        return _menuRepository.InsertAsync(menu);
    }

    /// <summary>
    /// Update a menu
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task UpdateMenuAsync(Menu menu)
    {
		_logger.LogError(">>> Entering UpdateMenuAsync(menu={Menu})", menu);
        _logger.LogWarning("<<< Exiting UpdateMenuAsync(menu={Menu})", menu);
        return _menuRepository.UpdateAsync(menu);
    }

    #endregion

    #region Menu items

    /// <summary>
    /// Deletes a menu item
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteMenuItemAsync(MenuItem menuItem)
    {
		_logger.LogError(">>> Entering DeleteMenuItemAsync(menuItem={MenuItem})", menuItem);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (menuItem is null)
            return;

        await deleteChildrenRecursive(menuItem.Id);
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeleteMenuItemAsync(menuItem)={MenuItem}: delete query executed in {Arg1} ms", menuItem, __logStartDb.ElapsedMilliseconds);
        await _menuItemRepository.DeleteAsync(menuItem);

        async Task deleteChildrenRecursive(int parentId)
        {
			_logger.LogError(">>> Entering deleteChildrenRecursive(parentId={ParentId})", parentId);
            var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var children = await GetAllMenuItemsAsync(parentMenuItemId: parentId, showHidden: true);
			_logger.LogWarning("<<< Exiting deleteChildrenRecursive(parentId={ParentId})", parentId);

			_logger.LogError("DeleteMenuItemAsync(menuItem={MenuItem}): children → {Children}", menuItem, children);
            foreach (var child in children)
            {
				_logger.LogError("deleteChildrenRecursive(parentId={ParentId}): children → {Children}", parentId, children);
				_logger.LogError("deleteChildrenRecursive(parentId)={ParentId}: delete query executed in {Arg1} ms", parentId, __logStartDb.ElapsedMilliseconds);
                await deleteChildrenRecursive(child.Id);
                await _menuItemRepository.DeleteAsync(child);
				_logger.LogError("deleteChildrenRecursive(parentId)={ParentId}: external service call took {Arg1} ms", parentId, __logStart.ElapsedMilliseconds);
            }
        }
		_logger.LogWarning("<<< Exiting DeleteMenuItemAsync(menuItem={MenuItem})", menuItem);
    }

    /// <summary>
    /// Gets a menu item by identifier
    /// </summary>
    /// <param name="menuItemId">The menu item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a menu item
    /// </returns>
    public virtual Task<MenuItem> GetMenuItemByIdAsync(int menuItemId)
    {
		_logger.LogError(">>> Entering GetMenuItemByIdAsync(menuItemId={MenuItemId})", menuItemId);
		_logger.LogError("GetMenuItemByIdAsync(menuItemId)={MenuItemId}: find query executing", menuItemId);
    _logger.LogWarning("<<< Exiting GetMenuItemByIdAsync(menuItemId={MenuItemId})", menuItemId);
        return _menuItemRepository.GetByIdAsync(menuItemId, cache => default);
    }

    /// <summary>
    /// Insert a menu item
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task InsertMenuItemAsync(MenuItem menuItem)
    {
		_logger.LogError(">>> Entering InsertMenuItemAsync(menuItem={MenuItem})", menuItem);
        _logger.LogWarning("<<< Exiting InsertMenuItemAsync(menuItem={MenuItem})", menuItem);
        return _menuItemRepository.InsertAsync(menuItem);
    }

    /// <summary>
    /// Update a menu item
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateMenuItemAsync(MenuItem menuItem)
    {
		_logger.LogError(">>> Entering UpdateMenuItemAsync(menuItem={MenuItem})", menuItem);
        ArgumentNullException.ThrowIfNull(menuItem);

        //validate hierarchy
        var parentMenuItem = await GetMenuItemByIdAsync(menuItem.ParentId);
		_logger.LogError("UpdateMenuItemAsync(menuItem={MenuItem}): parentMenuItem → {ParentMenuItem}", menuItem, parentMenuItem);
        while (parentMenuItem != null)
        {
            if (menuItem.Id == parentMenuItem.Id)
            {
                menuItem.ParentId = 0;
				_logger.LogDebug("UpdateMenuItemAsync(menuItem={MenuItem}): menuItem.ParentId → {Arg1}", menuItem, menuItem.ParentId);
                break;
            }

            parentMenuItem = await GetMenuItemByIdAsync(parentMenuItem.ParentId);
        }

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("UpdateMenuItemAsync(menuItem)={MenuItem}: update query executed in {Arg1} ms", menuItem, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting UpdateMenuItemAsync(menuItem={MenuItem})", menuItem);
        await _menuItemRepository.UpdateAsync(menuItem);
    }

    /// <summary>
    /// Get all menu items
    /// </summary>
    /// <param name="menuId">Menu identifier; 0 or null to load all records</param>
    /// <param name="parentMenuItemId">Parent menu item identifier</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="depth">Depth to limit items</param>
    /// <param name="treeSorting">A value indicating whether to sort menu items for tree representation</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains menu items
    /// </returns>
    public virtual async Task<IPagedList<MenuItem>> GetAllMenuItemsAsync(
        int menuId = 0,
        int parentMenuItemId = 0,
        int storeId = 0,
        int depth = 0,
        bool treeSorting = false,
        bool showHidden = false,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetAllMenuItemsAsync(menuId={MenuId},parentMenuItemId={ParentMenuItemId},storeId={StoreId},depth={Depth},treeSorting={TreeSorting},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", menuId, parentMenuItemId, storeId, depth, treeSorting, showHidden, pageIndex, pageSize);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var menuItems = await _menuItemRepository.GetAllAsync(async query =>
        {
            if (menuId > 0)
                query = query.Where(menuItem => menuItem.MenuId == menuId);

			_logger.LogError("GetAllMenuItemsAsync(menuId={MenuId},parentMenuItemId={ParentMenuItemId},storeId={StoreId},depth={Depth},treeSorting={TreeSorting},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", menuId, parentMenuItemId, storeId, depth, treeSorting, showHidden, pageIndex, pageSize, query);
            if (parentMenuItemId > 0)
                query = query.Where(mi => mi.ParentId == parentMenuItemId);

            if (!showHidden)
            {
                query = query.Where(m => m.Published);

                //apply ACL constraints
                var customer = await _workContext.GetCurrentCustomerAsync();
				_logger.LogError("GetAllMenuItemsAsync(menuId={MenuId},parentMenuItemId={ParentMenuItemId},storeId={StoreId},depth={Depth},treeSorting={TreeSorting},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize}): customer → {Customer}", menuId, parentMenuItemId, storeId, depth, treeSorting, showHidden, pageIndex, pageSize, customer);
                query = await _aclService.ApplyAcl(query, customer);
            }

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            return query
                .OrderBy(i => i.DisplayOrder)
                .ThenBy(i => i.Id);
        }, includeDeleted: false);
		_logger.LogError("GetAllMenuItemsAsync(menuId,parentMenuItemId,storeId,depth,treeSorting,showHidden,pageIndex,pageSize)={MenuId},{ParentMenuItemId},{StoreId},{Depth},{TreeSorting},{ShowHidden},{PageIndex},{PageSize}: find query executed in {Arg8} ms", menuId, parentMenuItemId, storeId, depth, treeSorting, showHidden, pageIndex, pageSize, __logStart.ElapsedMilliseconds);

        if (depth > 0)
            menuItems = FilterMenuItemsByDepth(menuItems, parentMenuItemId, depth).ToList();
			_logger.LogError("GetAllMenuItemsAsync(menuId={MenuId},parentMenuItemId={ParentMenuItemId},storeId={StoreId},depth={Depth},treeSorting={TreeSorting},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize}): menuItems → {MenuItems}", menuId, parentMenuItemId, storeId, depth, treeSorting, showHidden, pageIndex, pageSize, menuItems);

        if (treeSorting)
            menuItems = SortMenuItemsForTree(menuItems.ToLookup(c => c.ParentId)).ToList();

        //paging
		_logger.LogWarning("<<< Exiting GetAllMenuItemsAsync(menuId={MenuId},parentMenuItemId={ParentMenuItemId},storeId={StoreId},depth={Depth},treeSorting={TreeSorting},showHidden={ShowHidden},pageIndex={PageIndex},pageSize={PageSize})", menuId, parentMenuItemId, storeId, depth, treeSorting, showHidden, pageIndex, pageSize);
        return new PagedList<MenuItem>(menuItems, pageIndex, pageSize);
    }

    /// <summary>
    /// Ensure that menu items don't exceed a certain depth 
    /// </summary>
    /// <param name="menuItems">Menu items for filtering</param>
    /// <param name="parentId">Parent menu item identifier</param>
    /// <param name="depthLimit">Depth to limit items</param>
    /// <returns>
    /// Menu items limited in depth
    /// </returns>
    public virtual IEnumerable<MenuItem> FilterMenuItemsByDepth(IEnumerable<MenuItem> menuItems, int parentId = 0, int depthLimit = 1)
    {
		_logger.LogError(">>> Entering FilterMenuItemsByDepth(menuItems={MenuItems},parentId={ParentId},depthLimit={DepthLimit})", menuItems, parentId, depthLimit);
        ArgumentNullException.ThrowIfNull(menuItems);

        if (depthLimit == 0)
            yield break;

        depthLimit--;

        foreach (var item in menuItems.Where(item => item.ParentId == parentId))
        {
            yield return item;

            foreach (var child in FilterMenuItemsByDepth(menuItems, item.Id, depthLimit).OrderBy(item => item.DisplayOrder))
                yield return child;
        }
		_logger.LogWarning("<<< Exiting FilterMenuItemsByDepth(menuItems={MenuItems},parentId={ParentId},depthLimit={DepthLimit})", menuItems, parentId, depthLimit);
    }

    #endregion

    #endregion
}