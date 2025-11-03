package io.radar.sdk;

import android.content.Context;
import android.location.Location;
import io.radar.sdk.Radar.RadarStatus;
import io.radar.sdk.Radar.RadarLocationSource;
import io.radar.sdk.model.RadarUser;
import io.radar.sdk.RadarReceiver;
import io.radar.sdk.model.RadarEvent;

public class CustomReceiver extends RadarReceiver {

    // Define a listener interface for token updates
    public interface Listener {
        void onLog(Context context, String message);
        void onError(Context context, RadarStatus status);
    }

    private Listener listener;

    public CustomReceiver(Listener listener) {
        this.listener = listener;
    }

    @Override
    public void onLog(Context context, String message) {
        if (listener != null) {
            listener.onLog(context, message);
        }
    }

    @Override
    public void onError(Context context, RadarStatus status) {
        if (listener != null) {
            listener.onError(context, status);
        }
    }

    @Override
    public void onClientLocationUpdated(Context context, Location location, boolean stopped, RadarLocationSource source) {
        // if (listener != null) {
        //     listener.onClientLocationUpdated(context, location, stopped, source);
        // }
    }

    @Override
    public void onLocationUpdated(Context context, Location location, RadarUser user) {
        // if (listener != null) {
        //     listener.onLocationUpdated(context, location, user);
        // }
    }

    @Override
    public void onEventsReceived(Context context, RadarEvent[] events, RadarUser user) {
        // if (listener != null) {
        //     listener.onEventsReceived(context, events, user);
        // }
    }
}