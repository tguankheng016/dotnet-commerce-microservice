import { environment } from 'environments/environment';
import { enableProdMode } from '@angular/core';
import { RootModule } from 'root.module';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

if (environment.production) {
  enableProdMode();
}

const bootstrap = () => {
  return platformBrowserDynamic().bootstrapModule(RootModule);
};

bootstrap(); // Regular bootstrap
