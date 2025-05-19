import {Column, Entity, JoinColumn, ManyToOne, OneToOne, PrimaryColumn, PrimaryGeneratedColumn} from "typeorm";
import {Player} from "./Player";

@Entity()
export class HighScore {
    @PrimaryGeneratedColumn()
    id!: number;

    @ManyToOne(() => Player)
    @JoinColumn({
        name: 'player_id' ,
    })
    player!: Player;

    @Column({
        type: 'int',
        nullable: false,
    })
    highScore!: number;

    @Column({
        type: 'timestamp',
        nullable: false,
        default: () => 'CURRENT_TIMESTAMP',
    })
    timeStamp!: Date
}
