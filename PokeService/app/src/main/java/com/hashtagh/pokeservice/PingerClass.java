package com.hashtagh.pokeservice;

import android.app.Activity;
import android.content.Intent;
import android.widget.Toast;
import android.content.Context;
import android.util.Log;

public class PingerClass {
    public static Context context;

    public void initialize (Context _context, String token) {
        this.context = _context;
        Log.i ("Unity", "AAR Started");
        Intent serviceIntent = new Intent (context, BGService.class);
        serviceIntent.putExtra ("token", token);
        context.startService (serviceIntent);
    }
}