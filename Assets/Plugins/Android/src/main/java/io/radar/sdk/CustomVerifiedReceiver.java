package io.radar.sdk;

import android.content.Context;
import io.radar.sdk.model.RadarVerifiedLocationToken;
import io.radar.sdk.RadarVerifiedReceiver;

public class CustomVerifiedReceiver extends RadarVerifiedReceiver {

    // Define a listener interface for token updates
    public interface OnTokenUpdatedListener {
        void onTokenUpdated(Context context, RadarVerifiedLocationToken token);
    }

    private OnTokenUpdatedListener listener;

    public CustomVerifiedReceiver(OnTokenUpdatedListener listener) {
        this.listener = listener;
    }

    @Override
    public void onTokenUpdated(Context context, RadarVerifiedLocationToken token) {
        if (listener != null) {
            listener.onTokenUpdated(context, token);
        }
    }
}