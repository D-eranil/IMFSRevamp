import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ConfigData } from '../../models/misc/misc.model';
import { BroadcasterService } from './broadcaster.service';

@Injectable()

export class ConfigDataLoadedEvent {
    constructor(private broadcasterService: BroadcasterService) { }

    fire(data: any): void {
        this.broadcasterService.broadcast(ConfigDataLoadedEvent, data);
    }

    on(): Observable<ConfigData> {
        return this.broadcasterService.on<ConfigData>(ConfigDataLoadedEvent);
    }
}
