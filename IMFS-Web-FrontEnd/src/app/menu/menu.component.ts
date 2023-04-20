import { Component, OnInit, ViewChild } from '@angular/core';
import { MenuModule } from 'primeng/menu';
import { TabMenuModule } from 'primeng/tabmenu';
import { MenuItem } from 'primeng/api';

@Component({
    selector: 'app-menu',
    templateUrl: './menu.component.html'
})


export class MenuComponent implements OnInit {

    items: MenuItem[];
    activeItem: MenuItem;

    @ViewChild('menuItems') menu: MenuItem[];
    ngOnInit() {
        this.items = [
            { label: 'Home', icon: '' },
            { label: 'Quotes', icon: '' },
            { label: 'Applications', icon: '' },
            { label: 'Approvals', icon: '' },
            { label: 'Funder', icon: '', routerLink: ['/funder'] },
            {
                label: 'Settings',
                items: [{ label: 'admin' }, { label: 'funder' }]
            }
        ];

        this.activeItem = this.items[0];
    }

    activateMenu() {

    }


}
