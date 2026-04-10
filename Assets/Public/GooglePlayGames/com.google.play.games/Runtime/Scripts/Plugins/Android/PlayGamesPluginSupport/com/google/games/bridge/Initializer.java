package com.google.games.bridge;

import android.app.Activity;
import android.app.Instrumentation;
import android.util.Log;

import com.google.android.gms.games.PlayGamesSdk;

public final class Initializer {

    private static final String TAG = "GPGSInitializer";
    private static boolean initialized = false;

    private Initializer() {}

    public static boolean isInitialized() {
        return initialized;
    }

    public static void initialize(Activity activity) {
        if (initialized) {
            Log.d(TAG, "Already initialized, skipping");
            return;
        }

        if (activity == null) {
            Log.e(TAG, "Activity is null, cannot initialize PlayGamesSdk");
            return;
        }

        try {
            PlayGamesSdk.initialize(activity);
            initialized = true;
            Log.i(TAG, "PlayGamesSdk initialized successfully");

            activity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    try {
                        Instrumentation instrumentation = new Instrumentation();
                        instrumentation.callActivityOnPause(activity);
                        instrumentation.callActivityOnResume(activity);
                        Log.i(TAG, "Activity lifecycle callbacks re-triggered");
                    } catch (Exception e) {
                        Log.e(TAG, "Failed to re-trigger lifecycle callbacks: " + e.getMessage(), e);
                    }
                }
            });
        } catch (Exception e) {
            Log.e(TAG, "Failed to initialize PlayGamesSdk: " + e.getMessage(), e);
        }
    }
}
