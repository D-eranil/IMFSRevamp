import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TopNavEventData } from '../../models/misc/misc.model';
import { BroadcasterService } from '../../services/utility-services/broadcaster.service';

@Injectable()

export class TopNavEvent {
    constructor(private broadcasterService: BroadcasterService) { }

    fire(data: TopNavEventData): void {
        this.broadcasterService.broadcast(TopNavEvent, data);
    }

    on(): Observable<TopNavEventData> {
        return this.broadcasterService.on<TopNavEventData>(TopNavEvent);
    }
}

