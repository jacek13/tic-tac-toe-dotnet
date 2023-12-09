import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';
import { enableProdMode } from '@angular/core';
import { environment } from './environments/environment';

// TODO make environment file
if (environment.production) {
  enableProdMode();
}
console.log(`${ environment.production ? "prod" : "dev"}`)

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));