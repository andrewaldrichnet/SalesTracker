// Initialize IndexDB on app load
(async function initIndexDB() {
    try {
        const module = await import('/Data/indexeddb.js');
        const dbName = 'SalesTrackerDB';
        const dbVersion = 1;
        const storeNames = ['item', 'order', 'itemimage'];
        
        await module.initializeDatabase(dbName, dbVersion, storeNames);
        console.log('IndexDB initialized successfully with stores:', storeNames);
    } catch (error) {
        console.error('IndexDB initialization failed:', error);
    }
})();
