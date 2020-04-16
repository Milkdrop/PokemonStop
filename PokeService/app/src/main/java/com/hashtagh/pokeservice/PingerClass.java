package com.hashtagh.pokeservice;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Intent;
import android.content.Context;
import android.os.Build;
import android.util.Log;

public class PingerClass {
    public static Context context;
    public static Intent serviceIntent;

    public void initialize (Context _context, String token) {
        this.context = _context;
        Log.i ("Unity", "AAR Started");

        createNotificationChannel ();
        Log.i ("Unity", "Notification Channel created");

        serviceIntent = new Intent (context, BGService.class);
        serviceIntent.putExtra ("token", token);

        if (Build.VERSION.SDK_INT >= 26) {
            context.startForegroundService(serviceIntent);
        } else {
            context.startService(serviceIntent);
        }

        Log.i ("Unity", "Daemon started");
    }

    public void stop () {
        Log.i ("Unity", "User requested service stop");
        context.stopService (serviceIntent);
    }

    private void createNotificationChannel () {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationChannel serviceChannel = new NotificationChannel (
                    "stayHomeChannel",
                    "#StayHome Channel",
                    NotificationManager.IMPORTANCE_DEFAULT
            );

            NotificationManager manager = context.getSystemService (NotificationManager.class);
            manager.createNotificationChannel (serviceChannel);
        }
    }
}