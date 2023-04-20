import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd, RouterEvent } from '@angular/router';
import { AppComponent } from '../../app.component';
import { DatePipe } from '@angular/common';
import { MenuItem } from 'primeng/api';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { filter } from 'rxjs/operators';

@Component({
    selector: 'app-imfs-breadcrumb',
    templateUrl: 'imfs-breadcrumb.component.html',
    styleUrls: ['imfs-breadcrumb.component.scss'],
    encapsulation: ViewEncapsulation.None,
})

export class BreadcrumbComponent implements OnInit {

    static readonly ROUTE_DATA_BREADCRUMB = 'breadcrumb';
    // readonly home = { icon: 'pi pi-home', url: 'home' };
    readonly home = { label: 'IMFS', url: 'home' };
    menuItems: MenuItem[];

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        public app: AppComponent) {
    }

    ngOnInit() {
        this.router.events
            .pipe(filter(event => event instanceof NavigationEnd))
            .subscribe(() => this.menuItems = this.createBreadcrumbs(this.activatedRoute.root));

        // this.items = [
        //     { label: 'Home', routerLink: '/' },
        //     { label: 'Quotes', routerLink: '/quote' },
        //     { label: 'New Quote', routerLink: '/quote/new-quote' }
        // ];
    }

    private createBreadcrumbs(route: ActivatedRoute, url: string = '', breadcrumbs: MenuItem[] = []): MenuItem[] {

        try {
            const children: ActivatedRoute[] = route.children;

            if (children.length === 0) {
                return breadcrumbs;
            }

            for (const child of children) {
                const routeURL: string = child.snapshot.url.map(segment => segment.path).join('/');
                if (routeURL !== '') {
                    url += `/${routeURL}`;
                }

                if (child.snapshot.routeConfig != null) {
                    const label = child.snapshot.routeConfig.path;
                    if (label !== undefined || label != null) {
                        breadcrumbs.push({ label, url });
                    }
                }

                // const label = child.snapshot.data[BreadcrumbComponent.ROUTE_DATA_BREADCRUMB];

                // if (label === undefined || label == null) {
                //     breadcrumbs.push({ label, url });
                // }

                return this.createBreadcrumbs(child, url, breadcrumbs);
            }
        }
        catch (error) {
            console.error(error);
        }
        return breadcrumbs;
    }

    /*private createBreadcrumbs(route: ActivatedRoute, url: string = '#', breadcrumbs: MenuItem[] = []): MenuItem[] {

        try {
            const children: ActivatedRoute[] = route.children;

            if (children.length === 0) {
                return breadcrumbs;
            }

            for (const child of children) {
                const routeURL: string = child.snapshot.url.map(segment => segment.path).join('/');
                if (routeURL !== '') {
                    url += `/${routeURL}`;
                }

                if (child.snapshot.routeConfig != null) {
                    const label = child.snapshot.routeConfig.path;
                    if (label !== undefined || label != null) {
                        breadcrumbs.push({ label, url });
                    }
                }

                // const label = child.snapshot.data[BreadcrumbComponent.ROUTE_DATA_BREADCRUMB];

                // if (label === undefined || label == null) {
                //     breadcrumbs.push({ label, url });
                // }

                return this.createBreadcrumbs(child, url, breadcrumbs);
            }
        }
        catch (error) {
            console.error(error);
        }
        return breadcrumbs;
    }
    */


}
