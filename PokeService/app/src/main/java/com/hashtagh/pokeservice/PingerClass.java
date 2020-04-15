package com.hashtagh.pokeservice;

import android.app.Activity;
import android.content.Intent;
import android.widget.Toast;
import android.content.Context;

public class PingerClass {
    private Context context;

    public void setContext(Context _context) {
        this.context = _context;
    }

    public void startPluginService() {
        context.startService (new Intent (context, PingerClass.class));
    }

    public void showToast() {
        Toast.makeText (context, "this is my Toast message!", Toast.LENGTH_LONG).show ();
    }
}
