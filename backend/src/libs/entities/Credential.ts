import { Entity, PrimaryGeneratedColumn, Column, OneToOne, JoinColumn, PrimaryColumn } from 'typeorm';
import { Player } from './Player';

@Entity()
export class Credential {
  @PrimaryColumn({
    type: 'varchar',
    nullable: false,
    unique: true
  })
  id!: string;

  @Column({
    type: 'varchar',
    nullable: true
  })
  email!: string;

  @Column({
    type: 'varchar',
    nullable: true
  })
  password!: string;

  @OneToOne(() => Player, player => player.credential)
  @JoinColumn()
  player!: Player;
}
