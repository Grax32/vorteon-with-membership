import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { ServicesComponent } from './components/services/services.component';
import { PricingComponent } from './components/pricing/pricing.component';
import { AboutComponent } from './components/about/about.component';
import { ContactComponent } from './components/contact/contact.component';
import { GetStartedComponent } from './components/get-started/get-started.component';
import { DevelopersComponent } from './components/developers/developers.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'services', component: ServicesComponent },
    { path: 'pricing', component: PricingComponent },
    { path: 'about', component: AboutComponent },
    { path: 'contact', component: ContactComponent },
    { path: 'get-started', component: GetStartedComponent },
    { path: "developers", component: DevelopersComponent },
];
