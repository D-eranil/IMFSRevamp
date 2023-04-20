import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { filter, map } from 'rxjs/operators';

interface BroadcastEvent {
    key: any;
    data?: any;
}
@Injectable()
export class BroadcasterService {
    private eventBus: Subject<BroadcastEvent>;

    constructor() {
        this.eventBus = new Subject<BroadcastEvent>();
    }

    broadcast(key: any, data?: any) {
        setTimeout(() => this.eventBus.next({ key, data }), 0);
    }

    on<T>(key: any): Observable<T> {
        return this.eventBus.asObservable()
            .pipe(filter(event => event.key === key))
            .pipe(map(event => event.data as T));
    }
}
