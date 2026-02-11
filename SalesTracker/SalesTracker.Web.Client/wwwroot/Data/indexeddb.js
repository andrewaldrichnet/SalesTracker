/**
 * IndexedDB utility functions for CRUD operations
 * Provides a simple API for storing and retrieving data from IndexedDB
 */

let openedDatabases = {};
let allStoreNames = [];

/**
 * Initializes the database with all required object stores
 * @param {string} dbName - Database name
 * @param {number} version - Database version for schema management
 * @param {Array<string>} storeNames - Array of all object store names to create
 * @returns {Promise<void>}
 */
export async function initializeDatabase(dbName, version, storeNames) {
    // Store the list of all stores for use in openDatabase
    allStoreNames = storeNames;
    
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(dbName, version);

        request.onerror = () => {
            reject(new Error(`Failed to initialize IndexedDB: ${request.error}`));
        };

        request.onsuccess = () => {
            const db = request.result;
            const key = `${dbName}_${version}`;
            openedDatabases[key] = db;
            resolve();
        };

        request.onupgradeneeded = (event) => {
            const db = event.target.result;
            
            // Create all object stores during upgrade
            storeNames.forEach(storeName => {
                if (!db.objectStoreNames.contains(storeName)) {
                    const keyPath = storeName.charAt(0).toUpperCase() + storeName.slice(1) + 'ID';
                    const store = db.createObjectStore(storeName, { keyPath: keyPath, autoIncrement: true });
                    store.createIndex('createddate_idx', 'createddate', { unique: false });
                }
            });
        };
    });
}

/**
 * Opens or creates an IndexedDB database
 * @param {string} dbName - Database name
 * @param {number} version - Database version for schema management
 * @param {string} storeName - Object store name
 * @returns {Promise<IDBDatabase>}
 */
function openDatabase(dbName, version, storeName) {
    const key = `${dbName}_${version}`;
    
    if (openedDatabases[key]) {
        return Promise.resolve(openedDatabases[key]);
    }

    return new Promise((resolve, reject) => {
        const request = indexedDB.open(dbName, version);

        request.onerror = () => {
            reject(new Error(`Failed to open IndexedDB: ${request.error}`));
        };

        request.onsuccess = () => {
            const db = request.result;
            openedDatabases[key] = db;
            resolve(db);
        };

        request.onupgradeneeded = (event) => {
            const db = event.target.result;
            
            // Create object stores for all registered stores
            const stores = allStoreNames.length > 0 ? allStoreNames : [storeName];
            stores.forEach(name => {
                if (!db.objectStoreNames.contains(name)) {
                    const keyPath = name.charAt(0).toUpperCase() + name.slice(1) + 'ID';
                    const store = db.createObjectStore(name, { keyPath: keyPath, autoIncrement: true });
                    store.createIndex('createddate_idx', 'createddate', { unique: false });
                }
            });
        };
    });
}

/**
 * Retrieves all records from an object store
 * @param {string} dbName
 * @param {number} version
 * @param {string} storeName
 * @returns {Promise<string>} JSON array of records
 */
export async function getAll(dbName, version, storeName) {
    const db = await openDatabase(dbName, version, storeName);
    
    return new Promise((resolve, reject) => {
        try {
            const transaction = db.transaction([storeName], 'readonly');
            const store = transaction.objectStore(storeName);
            const request = store.getAll();

            request.onerror = () => {
                const errorMsg = request.error?.message || String(request.error);
                reject(new Error(`Failed to retrieve data from ${storeName}: ${errorMsg}`));
            };

            request.onsuccess = () => {
                resolve(JSON.stringify(request.result || []));
            };
            
            transaction.onerror = () => {
                const errorMsg = transaction.error?.message || String(transaction.error);
                reject(new Error(`Transaction failed for getAll on ${storeName}: ${errorMsg}`));
            };
        } catch (e) {
            reject(new Error(`Exception during getAll: ${e.message}`));
        }
    });
}

/**
 * Retrieves a single record by ID
 * @param {string} dbName
 * @param {number} version
 * @param {string} storeName
 * @param {number} id - Record ID (itemid or orderid)
 * @returns {Promise<string|null>} JSON object or null
 */
export async function getById(dbName, version, storeName, id) {
    const db = await openDatabase(dbName, version, storeName);
    
    return new Promise((resolve, reject) => {
        const transaction = db.transaction([storeName], 'readonly');
        const store = transaction.objectStore(storeName);
        const request = store.get(id);

        request.onerror = () => {
            reject(new Error(`Failed to retrieve record: ${request.error}`));
        };

        request.onsuccess = () => {
            resolve(request.result ? JSON.stringify(request.result) : null);
        };
    });
}

/**
 * Adds a new record to the object store
 * @param {string} dbName
 * @param {number} version
 * @param {string} storeName
 * @param {string} jsonData - JSON string of the object to add
 * @returns {Promise<number>} The key of the added record
 */
export async function add(dbName, version, storeName, jsonData) {
    const db = await openDatabase(dbName, version, storeName);
    let data;
    
    try {
        data = JSON.parse(jsonData);
    } catch (e) {
        throw new Error(`Failed to parse JSON data: ${e.message}`);
    }
    
    // Determine the key path for this store
    const keyPath = storeName.charAt(0).toUpperCase() + storeName.slice(1) + 'ID';
    
    // If the data has a key property with a value of 0 or a default value, 
    // delete it to allow auto-increment to work
    if (data[keyPath] === 0 || data[keyPath] === undefined) {
        delete data[keyPath];
    }
    
    return new Promise((resolve, reject) => {
        try {
            const transaction = db.transaction([storeName], 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.add(data);

            request.onerror = () => {
                const errorMsg = request.error?.message || String(request.error);
                console.error(`[IndexedDB Add Error] Store: ${storeName}, Error: ${errorMsg}`, data);
                reject(new Error(`Failed to add record to ${storeName}: ${errorMsg}`));
            };

            request.onsuccess = () => {
                console.log(`[IndexedDB Add Success] Store: ${storeName}, Key: ${request.result}`);
                resolve(request.result);
            };
            
            transaction.onerror = () => {
                const errorMsg = transaction.error?.message || String(transaction.error);
                console.error(`[IndexedDB Transaction Error] Store: ${storeName}, Error: ${errorMsg}`);
                reject(new Error(`Transaction failed for ${storeName}: ${errorMsg}`));
            };
        } catch (e) {
            console.error(`[IndexedDB Add Exception] Store: ${storeName}, Error: ${e.message}`);
            reject(new Error(`Exception during add operation: ${e.message}`));
        }
    });
}

/**
 * Updates an existing record
 * @param {string} dbName
 * @param {number} version
 * @param {string} storeName
 * @param {string} jsonData - JSON string of the object to update
 * @returns {Promise<number>} The key of the updated record
 */
export async function update(dbName, version, storeName, jsonData) {
    const db = await openDatabase(dbName, version, storeName);
    let data;
    
    try {
        data = JSON.parse(jsonData);
    } catch (e) {
        throw new Error(`Failed to parse JSON data: ${e.message}`);
    }
    
    return new Promise((resolve, reject) => {
        try {
            const transaction = db.transaction([storeName], 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.put(data);

            request.onerror = () => {
                const errorMsg = request.error?.message || String(request.error);
                reject(new Error(`Failed to update record in ${storeName}: ${errorMsg}`));
            };

            request.onsuccess = () => {
                resolve(request.result);
            };
            
            transaction.onerror = () => {
                const errorMsg = transaction.error?.message || String(transaction.error);
                reject(new Error(`Transaction failed for update on ${storeName}: ${errorMsg}`));
            };
        } catch (e) {
            reject(new Error(`Exception during update: ${e.message}`));
        }
    });
}

/**
 * Deletes a record by ID
 * @param {string} dbName
 * @param {number} version
 * @param {string} storeName
 * @param {number} id - Record ID to delete
 * @returns {Promise<void>}
 */
export async function deleteRecord(dbName, version, storeName, id) {
    const db = await openDatabase(dbName, version, storeName);
    
    return new Promise((resolve, reject) => {
        try {
            const transaction = db.transaction([storeName], 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.delete(id);

            request.onerror = () => {
                const errorMsg = request.error?.message || String(request.error);
                reject(new Error(`Failed to delete record from ${storeName}: ${errorMsg}`));
            };

            request.onsuccess = () => {
                resolve();
            };
            
            transaction.onerror = () => {
                const errorMsg = transaction.error?.message || String(transaction.error);
                reject(new Error(`Transaction failed for delete on ${storeName}: ${errorMsg}`));
            };
        } catch (e) {
            reject(new Error(`Exception during delete: ${e.message}`));
        }
    });
}

/**
 * Clears all records from an object store
 * @param {string} dbName
 * @param {number} version
 * @param {string} storeName
 * @returns {Promise<void>}
 */
export async function clearStore(dbName, version, storeName) {
    const db = await openDatabase(dbName, version, storeName);
    
    return new Promise((resolve, reject) => {
        const transaction = db.transaction([storeName], 'readwrite');
        const store = transaction.objectStore(storeName);
        const request = store.clear();

        request.onerror = () => {
            reject(new Error(`Failed to clear store: ${request.error}`));
        };

        request.onsuccess = () => {
            resolve();
        };
    });
}
