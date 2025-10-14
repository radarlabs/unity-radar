mergeInto(LibraryManager.library, {
    
    Radar_initializeWithPublishableKey: function(publishableKey) {
        var key = UTF8ToString(publishableKey);

        console.log('Initializing Radar with publishable key:', key);
        
        try {
            window.Radar.initialize(key);
            console.log('Radar Web SDK initialized with key:', key);
        } catch (error) {
            console.error('Failed to initialize Radar SDK:', error);
        }
    },

    Radar_setDelegateCallbacks: function(logCallback, errorCallback, tokenUpdatedCallback) {
        // Listen for Radar SDK console logs
        (function() {
            var origLog = console.log;
            console.log = function(msg) {
                var prefixes = ["Radar SDK: ", "Radar SDK (debug): "];
                for (var i = 0; i < prefixes.length; i++) {
                    var prefix = prefixes[i];
                    if (typeof msg === "string" && msg.startsWith(prefix)) {
                        var trimmed = msg.slice(prefix.length);
                        var buffer = stringToNewUTF8(trimmed);
                        {{{ makeDynCall('vi', 'logCallback') }}}(buffer);
                        _free(buffer);
                        break;
                    }
                }
                origLog.apply(console, arguments);
            };
        })();

        window.Radar.onError(function(status) {
            const buffer = stringToNewUTF8(status);
            {{{ makeDynCall('vi', 'errorCallback') }}} (buffer);
            _free(buffer);
        });

        window.Radar.onTokenUpdated(function(token) {
            const buffer = stringToNewUTF8(JSON.stringify(token));
            {{{ makeDynCall('vi', 'tokenUpdatedCallback') }}} (buffer);
            _free(buffer);
        });
    },
    
    Radar_setUserId: function(userId) {
        var id = UTF8ToString(userId);
        try {
            window.Radar.setUserId(id);
            console.log('User ID set to:', id);
        } catch (error) {
            console.error('Failed to set user ID:', error);
        }
    },
    
    Radar_setMetadata: function(metadataJson) {
        var metadata = UTF8ToString(metadataJson);
        try {
            var metadataObj = JSON.parse(metadata);
            window.Radar.setMetadata(metadataObj);
            console.log('Metadata set:', metadataObj);
        } catch (error) {
            console.error('Failed to set metadata:', error);
        }
    },
    
    Radar_requestLocationPermissions: function() {
        console.log('Requesting location permissions...');
        
        // Radar Web SDK handles permissions internally
        // We can trigger a location request to prompt for permissions
        window.Radar.getLocation()
        // .then(function(position) {
        //     console.log('Location permission granted');
        //     const buffer = stringToNewUTF8('Location permission granted');
        //     {{{ makeDynCall('vi', 'logCallback') }}} (buffer);
        //     _free(buffer);
        // }.bind(this)).catch(function(error) {
        //     console.error('Location permission denied:', error);
        //     const buffer = stringToNewUTF8('ERROR_PERMISSIONS');
        //     {{{ makeDynCall('vi', 'errorCallback') }}} (buffer);
        //     _free(buffer);
        // }.bind(this));
    },
    
    Radar_getVerifiedLocationToken: function(requestId, callback) {
        window.Radar.getVerifiedLocationToken().then(function(response) {
            if (response) {
                var jsonStr = JSON.stringify(response);
                const statusBuffer = stringToNewUTF8('SUCCESS');
                const jsonBuffer = stringToNewUTF8(jsonStr);
                {{{ makeDynCall('viii', 'callback') }}} (requestId, statusBuffer, jsonBuffer);
                _free(statusBuffer);
                _free(jsonBuffer);
            } else {
                const statusBuffer = stringToNewUTF8('ERROR_UNKNOWN');
                const jsonBuffer = stringToNewUTF8('');
                {{{ makeDynCall('viii', 'callback') }}} (requestId, statusBuffer, jsonBuffer);
                _free(statusBuffer);
                _free(jsonBuffer);
            }
        }).catch(function(error) {
            console.error('Failed to get verified location token:', error);
            var status = error.message || 'ERROR_UNKNOWN';
            const statusBuffer = stringToNewUTF8(status);
            const jsonBuffer = stringToNewUTF8('');
            {{{ makeDynCall('viii', 'callback') }}} (requestId, statusBuffer, jsonBuffer);
            _free(statusBuffer);
            _free(jsonBuffer);
        });
    },
    
    Radar_trackVerified: function(requestId, callback, desiredAccuracy) {
        var accuracy = UTF8ToString(desiredAccuracy);
        
        // Convert accuracy string to Radar SDK format
        var accuracyOptions = {
            'HIGH': { accuracy: 'high' },
            'MEDIUM': { accuracy: 'medium' },
            'LOW': { accuracy: 'low' },
            'NONE': { accuracy: 'none' }
        };
        
        var params = accuracyOptions[accuracy] || { accuracy: 'medium' };
        
        window.Radar.trackVerified(params)
        .then(function(response) {
            var jsonStr = JSON.stringify(response);
            const statusBuffer = stringToNewUTF8('SUCCESS');
            const jsonBuffer = stringToNewUTF8(jsonStr);
            console.log('Track verified response:', jsonStr);
            {{{ makeDynCall('viii', 'callback') }}} (requestId, statusBuffer, jsonBuffer);
            _free(statusBuffer);
            _free(jsonBuffer);
        }).catch(function(error) {
            console.error('Failed to track verified:', error);
            var status = 'ERROR_UNKNOWN';
            const statusBuffer = stringToNewUTF8(status);
            const jsonBuffer = stringToNewUTF8('');
            {{{ makeDynCall('viii', 'callback') }}} (requestId, statusBuffer, jsonBuffer);
            _free(statusBuffer);
            _free(jsonBuffer);
        });
    },
    
    Radar_startTrackingVerified: function(interval, beacons) {
        console.log('Starting verified tracking with interval:', interval, 'beacons:', beacons);
        
        try {
            window.Radar.startTrackingVerified({
                interval: interval,
                beacons: beacons
            });
        } catch (error) {
            console.error('Failed to start tracking verified:', error);
        }
    },
    
    Radar_stopTrackingVerified: function() {
        console.log('Stopping verified tracking');
        
        try {
            window.Radar.stopTrackingVerified();
        } catch (error) {
            console.error('Failed to stop tracking verified:', error);
        }
    },
    
    Radar_getLocation: function(requestId, callback) {
        window.Radar.getLocation().then(function(position) {
            var jsonStr = JSON.stringify({
                coordinates: [position.longitude, position.latitude],
                accuracy: position.accuracy
            });
            const statusBuffer = stringToNewUTF8('SUCCESS');
            const jsonBuffer = stringToNewUTF8(jsonStr);
            {{{ makeDynCall('viii', 'callback') }}} (requestId, statusBuffer, jsonBuffer);
            _free(statusBuffer);
            _free(jsonBuffer);
        }).catch(function(error) {
            console.error('Failed to get location:', error);
            var status = 'ERROR_LOCATION';
            const statusBuffer = stringToNewUTF8(status);
            const jsonBuffer = stringToNewUTF8('');
            {{{ makeDynCall('viii', 'callback') }}} (requestId, statusBuffer, jsonBuffer);
            _free(statusBuffer);
            _free(jsonBuffer);
        });
    }
});
